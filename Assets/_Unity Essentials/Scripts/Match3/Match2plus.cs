using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using OutlineFx;
using System.Linq;

namespace Match3
{
    public class Match2Plus : MonoBehaviour
    {
        [Header("Grid Size & Placing")]
        [SerializeField] int width = 8;
        [SerializeField] int height = 8;
        [SerializeField] float cellSize = 1f;
        [SerializeField] Vector3 originPosition = Vector3.zero;
        [SerializeField] bool gridAutoCenter = false;
        [SerializeField] bool debug = true;

        [Header("Items & types")]
        [SerializeField] Gem gemPrefab;
        [SerializeField] GemType[] gemTypes;

        [Header("Action Settings")]
        [SerializeField] float gemSwapTime = 0.5f;
        [SerializeField] Ease gemSwapEase = Ease.InQuad;
        [SerializeField] float gemDropTime = 0.5f;
        [SerializeField] Ease gemDropEase = Ease.InQuad;
        [SerializeField, Range(0f, 1f)] float gemDropWaitFactor = 0.75f;
        [SerializeField] GameObject gemPopVFX;
        [SerializeField, Range(0.5f, 3f)] float popFXScaleFactor = 1.5f;


        private GridSystem2D<GridObject<Gem>> grid;
        private Vector2Int selectedGem = Vector2Int.one * -1;
        private InputReader inputReader;
        private AudioManager audioManager;

        private void Awake()
        {
            inputReader = GetComponent<InputReader>();
            audioManager = GetComponent<AudioManager>();
        }

        void Start()
        {
            GridAutoCenter();
            InitializeGrid();

            inputReader.Fire += OnSelectGem;
        }

        private void OnDestroy()
        {
            inputReader.Fire -= OnSelectGem;
        }

        private void OnSelectGem()
        {
            var gridPosition = grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

            // click out of grid => deselect
            var clickOutOfGrid = !grid.IsValid(gridPosition.x, gridPosition.y);
            if (clickOutOfGrid)
            {
                DeselectGem();
                return;
            }

            // click the empty slot (for a chance there's one) => ignore
            if (grid.IsEmpty(gridPosition.x, gridPosition.y))
            {
                return;
            }

            if (selectedGem == Vector2Int.one * -1)
            {
                SelectGem(gridPosition);
                StartCoroutine(RunMatchLoop());
            }

        }

        private void SetOutline(Vector2Int gridPosition, bool enabled)
        {
            var gemOutline = grid.GetValue(gridPosition.x, gridPosition.y)?.GetValue()?.gameObject?.GetComponent<Outline>();
            if (gemOutline != null) gemOutline.enabled = enabled;
        }

        private void SelectGem(Vector2Int gridPosition)
        {
            selectedGem = gridPosition;
            SetOutline(gridPosition, true);
            audioManager.PlaySelect();
        }

        private void DeselectGem()
        {
            SetOutline(selectedGem, false);
            selectedGem = Vector2Int.one * -1;
        }

        IEnumerator RunMatchLoop()
        {
            HashSet<Vector2Int> matches = FindGroupMatches();

            if (matches.Count >= 2)
            {
                yield return StartCoroutine(ExplodeGems(matches));

                yield return StartCoroutine(MakeGemsFall());

                yield return StartCoroutine(FillEmptySpots());
            }

            DeselectGem();

            yield return null;
        }

        private IEnumerator FillEmptySpots()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid.IsEmpty(x, y))
                    {
                        CreateGem(x, y);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }

        private IEnumerator MakeGemsFall()
        {
            for (int x = 0; x < width; x++)
            {
                int emptyY = -1; // Позиція першої пустої клітинки в колонці
                for (int y = 0; y < height; y++)
                {
                    if (grid.GetValue(x, y) == null)
                    {
                        if (emptyY == -1)
                            emptyY = y; // Зберігаємо першу пусту клітинку
                    }
                    else if (emptyY != -1)
                    {
                        // Переміщуємо перший доступний камінь у першу пусту позицію
                        var gem = grid.GetValue(x, y).GetValue();
                        grid.SetValue(x, emptyY, grid.GetValue(x, y));
                        grid.SetValue(x, y, null);

                        // Анімація переміщення
                        gem.transform.DOLocalMove(grid.GetWorldPositionCenter(x, emptyY), gemDropTime).SetEase(gemDropEase);
                        audioManager.PlayDrop();

                        yield return new WaitForSeconds(gemDropTime * gemDropWaitFactor);

                        emptyY++; // Зсуваємо пусту позицію вгору
                    }
                }
            }
        }

        private IEnumerator ExplodeGems(HashSet<Vector2Int> matches)
        {
            foreach (var match in matches)
            {
                var gem = grid.GetValue(match.x, match.y).GetValue();
                grid.SetValue(match.x, match.y, null);

                ExplodeFX(match, gem.GetGemType());

                gem.transform.DOPunchScale(Vector3.one * 0.1f, duration: 0.1f, vibrato: 1, elasticity: 0.5f);

                yield return new WaitForSeconds(0.1f);

                gem.CollectGem();
            }
        }

        private void ExplodeFX(Vector2Int gemLocation, GemType gemType)
        {
            // TODO: FX Pool
            var vfx = Instantiate(gemPopVFX, grid.GetWorldPositionCenter(gemLocation.x, gemLocation.y), Quaternion.Euler(grid.GetForward()), transform);

            var particlesMain = vfx.GetComponent<ParticleSystem>().main;
            particlesMain.startColor = new ParticleSystem.MinMaxGradient(gemType.color);
            vfx.transform.localScale = Vector3.one * popFXScaleFactor;

            audioManager.PlayPop();
        }

        private HashSet<Vector2Int> FindGroupMatches()
        {
            var neededType = grid.GetValue(selectedGem.x, selectedGem.y).GetValue().GetGemType();

            HashSet<Vector2Int> oldMatches = new();

            HashSet<Vector2Int> newlyFoundMatches = new() { selectedGem };

            HashSet<Vector2Int> batchToCheck = new();

            int searchPassNo = 0;

            while (newlyFoundMatches.Count > 0)
            {
                searchPassNo++;

                batchToCheck.Clear();
                batchToCheck.UnionWith(newlyFoundMatches);

                foreach (var match in batchToCheck)
                {
                    SetOutline(match, true);

                    // add the processed match to oldmatches
                    oldMatches.Add(match);
                    newlyFoundMatches.Remove(match);

                    // get spots on 4 sides around match
                    HashSet<Vector2Int> spotsAround = new();

                    if (match.x - 1 >= 0) spotsAround.Add(new(match.x - 1, match.y));
                    if (match.x + 1 < width) spotsAround.Add(new(match.x + 1, match.y));
                    if (match.y - 1 >= 0) spotsAround.Add(new(match.x, match.y - 1));
                    if (match.y + 1 < height) spotsAround.Add(new(match.x, match.y + 1));

                    // exclude the ones already in matches
                    spotsAround.ExceptWith(oldMatches);
                    if (spotsAround.Count == 0) continue;

                    // exclude the ones already in checks
                    spotsAround.ExceptWith(newlyFoundMatches);
                    if (spotsAround.Count == 0) continue;

                    // filter by being a gem
                    spotsAround.RemoveWhere(s => grid.GetValue(s.x, s.y).GetValue() is not Gem);
                    if (spotsAround.Count() == 0) continue;

                    // filter by gemtype
                    spotsAround.RemoveWhere(s => grid.GetValue(s.x, s.y).GetValue().GetGemType() != neededType);
                    if (spotsAround.Count() == 0) continue;

                    // if what's left > 0, add it to newly found matches
                    newlyFoundMatches.UnionWith(spotsAround);

                }

            }

            if (oldMatches.Count == 0)
            {
                audioManager.PlayNoMatch();
            }
            else
            {
                audioManager.PlayMatch();
            }

            return oldMatches;
        }

        IEnumerator SwapGems(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            var gridObjectA = grid.GetValue(gridPositionA.x, gridPositionA.y);
            var gridObjectB = grid.GetValue(gridPositionB.x, gridPositionB.y);

            gridObjectA.GetValue().transform
                .DOLocalMove(grid.GetWorldPositionCenter(gridPositionB.x, gridPositionB.y), duration: gemSwapTime)
                .SetEase(gemSwapEase);
            gridObjectB.GetValue().transform
                .DOLocalMove(grid.GetWorldPositionCenter(gridPositionA.x, gridPositionA.y), duration: gemSwapTime)
                .SetEase(gemSwapEase);

            grid.SetValue(gridPositionA.x, gridPositionA.y, gridObjectB);
            grid.SetValue(gridPositionB.x, gridPositionB.y, gridObjectA);

            yield return new WaitForSeconds(gemSwapTime);

            SetOutline(gridPositionA, false);
            SetOutline(gridPositionB, false);

            yield return StartCoroutine(RunMatchLoop());
        }

        void GridAutoCenter()
        {
            if (gridAutoCenter)
            {
                originPosition.x = -width * 0.5f;
                originPosition.y = -height * 0.5f;
            }
        }

        void InitializeGrid()
        {
            grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(width, height, cellSize, originPosition, debug);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CreateGem(x, y, silently: true);
                }
            }

            //StartCoroutine(RunMatchLoop());
        }

        private void CreateGem(int x, int y, bool silently = false)
        {
            if (!silently) audioManager.PlayCreate();

            Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            gem.SetGemType(gemTypes[Random.Range(0, gemTypes.Length)]);

            var gridObject = new GridObject<Gem>(grid, x, y);
            gridObject.SetValue(gem);
            grid.SetValue(x, y, gridObject);
        }
    }
}
