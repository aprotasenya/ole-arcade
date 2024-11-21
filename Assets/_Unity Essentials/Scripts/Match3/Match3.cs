using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using OutlineFx;

namespace Match3
{
    public class Match3 : MonoBehaviour
    {
        [Header("Grid & Gem Settings")]
        [SerializeField] int width = 8;
        [SerializeField] int height = 8;
        [SerializeField] float cellSize = 1f;
        [SerializeField] Vector3 originPosition = Vector3.zero;
        [SerializeField] bool gridAutoCenter = false;
        [SerializeField] bool debug = true;
        [SerializeField] GridItem gemPrefab;
        [SerializeField] GridItemType[] gemTypes;

        [Header("Action Settings")]
        [SerializeField] float gemSwapTime = 0.5f;
        [SerializeField] Ease gemSwapEase = Ease.InQuad;
        [SerializeField] float gemDropTime = 0.5f;
        [SerializeField] Ease gemDropEase = Ease.InQuad;
        [SerializeField, Range(0f, 1f)] float gemDropWaitFactor = 0.75f;
        [SerializeField] GameObject gemPopVFX;
        [SerializeField, Range(0.5f, 3f)] float popFXScaleFactor = 1.5f;


        private GridSystem2D<GridObject<GridItem>> grid;
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

            if (selectedGem == gridPosition)
            {
                DeselectGem();
            }
            else if (selectedGem == Vector2Int.one * -1)
            {
                SelectGem(gridPosition);
                SetOutline(gridPosition, true);
                audioManager.PlaySelect();
            }
            else
            {
                // TODO: Limit to only neighbour swaps?
                // TODO: Limit valid moves only to matching ones?

                SetOutline(gridPosition, true);
                audioManager.PlaySelect();
                StartCoroutine(SwapGems(selectedGem, gridPosition));
                DeselectGem();
            }
        }

        private void SetOutline(Vector2Int gridPosition, bool enabled)
        {
            var gemOutline = grid.GetObject(gridPosition.x, gridPosition.y)?.GetItem()?.gameObject?.GetComponent<Outline>();
            if (gemOutline != null) gemOutline.enabled = enabled;
        }

        private void SelectGem(Vector2Int gridPosition)
        {
            selectedGem = gridPosition;
        }

        private void DeselectGem()
        {
            SetOutline(selectedGem, false);
            selectedGem = Vector2Int.one * -1;
        }

        IEnumerator RunMatchLoop()
        {
            HashSet<Vector2Int> matches = FindLinearMatches();

            while (matches.Count > 0)
            {
                yield return StartCoroutine(ExplodeGems(matches));

                // TODO: Calculate score

                yield return StartCoroutine(MakeGemsFall());

                yield return StartCoroutine(FillEmptySpots());

                matches = FindLinearMatches();
            }

            // TODO: Check for GameOver

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
                    if (grid.GetObject(x, y) == null)
                    {
                        if (emptyY == -1)
                            emptyY = y; // Зберігаємо першу пусту клітинку
                    }
                    else if (emptyY != -1)
                    {
                        // Переміщуємо перший доступний камінь у першу пусту позицію
                        var gem = grid.GetObject(x, y).GetItem();
                        grid.SetObject(x, emptyY, grid.GetObject(x, y));
                        grid.SetObject(x, y, null);

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
                var gem = grid.GetObject(match.x, match.y).GetItem();
                grid.SetObject(match.x, match.y, null);

                ExplodeFX(match, gem.GetItemType());

                gem.transform.DOPunchScale(Vector3.one * 0.1f, duration: 0.1f, vibrato: 1, elasticity: 0.5f);

                yield return new WaitForSeconds(0.1f);

                gem.CollectItem();
            }
        }

        private void ExplodeFX(Vector2Int gemLocation, GridItemType gemType)
        {
            // TODO: FX Pool
            var vfx = Instantiate(gemPopVFX, grid.GetWorldPositionCenter(gemLocation.x, gemLocation.y), Quaternion.Euler(grid.GetForward()), transform);

            var particlesMain = vfx.GetComponent<ParticleSystem>().main;
            particlesMain.startColor = new ParticleSystem.MinMaxGradient(gemType.color);
            vfx.transform.localScale = Vector3.one * popFXScaleFactor;

            audioManager.PlayPop();
        }

        private HashSet<Vector2Int> FindLinearMatches()
        {
            HashSet<Vector2Int> matches = new();

            // Helper function for checking a line (horizontal or vertical)
            void CheckLine(int startX, int startY, int dirX, int dirY)
            {
                for (int i = 0; i < (dirX == 1 ? width : height) - 2; i++)
                {
                    var gemA = grid.GetObject(startX + i * dirX, startY + i * dirY);
                    var gemB = grid.GetObject(startX + (i + 1) * dirX, startY + (i + 1) * dirY);
                    var gemC = grid.GetObject(startX + (i + 2) * dirX, startY + (i + 2) * dirY);

                    if (gemA == null || gemB == null || gemC == null) continue;

                    var gemTypeA = gemA.GetItem().GetItemType();
                    var gemTypeB = gemB.GetItem().GetItemType();
                    var gemTypeC = gemC.GetItem().GetItemType();

                    if (gemTypeA == gemTypeB && gemTypeB == gemTypeC)
                    {
                        matches.Add(new Vector2Int(startX + i * dirX, startY + i * dirY));
                        matches.Add(new Vector2Int(startX + (i + 1) * dirX, startY + (i + 1) * dirY));
                        matches.Add(new Vector2Int(startX + (i + 2) * dirX, startY + (i + 2) * dirY));
                    }
                }
            }

            // Horizontal matches
            for (int y = 0; y < height; y++)
            {
                CheckLine(0, y, 1, 0); // Check horizontally
            }

            // Vertical matches
            for (int x = 0; x < width; x++)
            {
                CheckLine(x, 0, 0, 1); // Check vertically
            }

            if (matches.Count == 0)
            {
                audioManager.PlayNoMatch();
            }
            else
            {
                audioManager.PlayMatch();
            }

            return matches;
        }

        IEnumerator SwapGems(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            var gridObjectA = grid.GetObject(gridPositionA.x, gridPositionA.y);
            var gridObjectB = grid.GetObject(gridPositionB.x, gridPositionB.y);

            gridObjectA.GetItem().transform
                .DOLocalMove(grid.GetWorldPositionCenter(gridPositionB.x, gridPositionB.y), duration: gemSwapTime)
                .SetEase(gemSwapEase);
            gridObjectB.GetItem().transform
                .DOLocalMove(grid.GetWorldPositionCenter(gridPositionA.x, gridPositionA.y), duration: gemSwapTime)
                .SetEase(gemSwapEase);

            grid.SetObject(gridPositionA.x, gridPositionA.y, gridObjectB);
            grid.SetObject(gridPositionB.x, gridPositionB.y, gridObjectA);

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
            grid = GridSystem2D<GridObject<GridItem>>.VerticalGrid(width, height, cellSize, originPosition, debug);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CreateGem(x, y, silently: true);
                }
            }

            StartCoroutine(RunMatchLoop());
        }

        private void CreateGem(int x, int y, bool silently = false)
        {
            if (!silently) audioManager.PlayCreate();

            GridItem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            gem.SetItemType(gemTypes[Random.Range(0, gemTypes.Length)]);

            var gridObject = new GridObject<GridItem>(grid, x, y);
            gridObject.SetItem(gem);
            grid.SetObject(x, y, gridObject);
        }
    }
}
