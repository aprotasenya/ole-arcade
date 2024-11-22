using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using OutlineFx;
using System.Linq;
using System;
using Random = UnityEngine.Random;

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
        [SerializeField] GridItem gemPrefab;
        [SerializeField] GridItemType blankItemType;
        [SerializeField] GridItemType sandStoneType;
        [SerializeField] float sandStonesAtTheBottom = 0f;
        [SerializeField] GridItemType[] gemTypes;

        [Header("Action Settings")]
        [SerializeField, Range(0f, 1f)] float pauseBetweenSteps = 0.2f;
        [SerializeField, Range(0f, 1f)] float pauseAfterCreateItem = 0.2f;
        [SerializeField, Range(0f, 1f)] float gemDropTime = 0.5f;
        [SerializeField] Ease gemDropEase = Ease.InQuad;
        [SerializeField, Range(0f, 1f)] float gemDropWaitFactor = 0.75f;
        [SerializeField] GameObject gemPopVFX;
        [SerializeField, Range(0.5f, 2f)] float popFXScaleFactor = 1.5f;


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

            inputReader.Fire += OnClickItem;
        }

        private void OnDestroy()
        {
            inputReader.Fire -= OnClickItem;
        }

        private void OnClickItem()
        {
            var gridPosition = grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

            // click out of grid => deselect
            if (!grid.IsValid(gridPosition.x, gridPosition.y))
            {
                DeselectGem();
                return;
            }

            // click the empty slot (for a chance there's one) => ignore
            if (grid.IsEmpty(gridPosition.x, gridPosition.y)) return;

            // click uninteractable => ignore
            if (!grid.GetObject(gridPosition.x, gridPosition.y).GetItem().IsClickable) return;


            if (selectedGem == Vector2Int.one * -1)
            {
                SelectGem(gridPosition);
                StartCoroutine(RunMatchLoop());
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
            audioManager.PlaySelect();
        }

        private void DeselectGem()
        {
            selectedGem = Vector2Int.one * -1;
        }

        IEnumerator RunMatchLoop()
        {
            HashSet<Vector2Int> matches = FindGroupMatches();

            if (matches.Count >= 2)
            {
                audioManager.PlayMatch();

                yield return StartCoroutine(ExplodeGems(matches));
                yield return new WaitForSeconds(pauseBetweenSteps);

                var destructibles = FindDestructiblesAndBlanks(matches);

                if (destructibles.Count > 0)
                {
                    yield return StartCoroutine(ExplodeGems(destructibles));
                    yield return new WaitForSeconds(pauseBetweenSteps);
                }

                yield return StartCoroutine(MakeGemsFall());
                //yield return new WaitForSeconds(pauseBetweenSteps);

                DeselectGem();

                yield return StartCoroutine(FillEmptySpots());
                //yield return new WaitForSeconds(pauseBetweenSteps);
            }
            else
            {
                DeselectGem();
            }

            yield return null;
        }

        private HashSet<Vector2Int> FindDestructiblesAndBlanks(HashSet<Vector2Int> matches)
        {
            HashSet<Vector2Int> allNeighbours = new();

            foreach (var match in matches)
            {
                allNeighbours.UnionWith(grid.GetAdjacentCoordinates(match.x, match.y));

            }

            HashSet<Vector2Int> destructibles = allNeighbours
                .Except(matches)
                .Where(n => grid.GetObject(n.x, n.y).GetItem().IsNearDestructible)
                .ToHashSet();

            if (destructibles.Where(d => d.y > 0).Count() == 0) return destructibles;

            HashSet<Vector2Int> blanks = new();

            foreach (var stone in destructibles)
            {
                if (stone.y == 0) continue;

                for (int i = stone.y - 1; i >= 0; i--)
                {
                    if (grid.GetObject(stone.x, i)?.GetItem()?.GetItemType() != blankItemType) break;

                    blanks.Add(new(stone.x, i));
                }
            }

            if (blanks.Count > 0) destructibles.UnionWith(blanks);

            return destructibles;

        }

        private IEnumerator FillEmptySpots()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid.IsEmpty(x, y))
                    {
                        CreateItem(x, y);
                        yield return new WaitForSeconds(pauseAfterCreateItem);
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
                    if (grid.IsEmpty(x, y))
                    {
                        if (emptyY == -1)
                            emptyY = y; // Зберігаємо першу пусту клітинку
                    }
                    else if (emptyY != -1)
                    {
                        if (!grid.GetObject(x, y).GetItem().GetItemType().IsMovable)
                        {
                            for (int i = emptyY; i < y; i++)
                            {
                                CreateItem(x, i, blankItemType, true);
                            }
                            yield return new WaitForSeconds(gemDropTime * gemDropWaitFactor);

                            emptyY = -1;
                        }
                        else
                        {
                            // Переміщуємо перший доступний камінь у першу пусту позицію
                            var gem = grid.GetObject(x, y).GetItem();
                            grid.SetObject(x, emptyY, grid.GetObject(x, y));
                            grid.SetObject(x, y, null);

                            // Анімація переміщення
                            gem.transform.DOLocalMove(grid.GetWorldPositionCenter(x, emptyY), gemDropTime).SetEase(gemDropEase);
                            yield return new WaitForSeconds(gemDropTime * gemDropWaitFactor);

                            emptyY++; // Зсуваємо пусту позицію вгору
                        }

                    }
                }
                if (emptyY != -1) audioManager.PlayDrop();

            }
        }

        private IEnumerator ExplodeGems(HashSet<Vector2Int> matches)
        {
            foreach (var match in matches)
            {
                var gem = grid.GetObject(match.x, match.y).GetItem();
                grid.SetObject(match.x, match.y, null);

                ExplodeFX(match, gem.GetItemType());

                gem.CollectItem();
            }

            audioManager.PlayPop();

            yield return null;
        }

        // TODO: Transfer this to Items (and give gems and stones different VFX/SFX)
        private void ExplodeFX(Vector2Int gemLocation, GridItemType gemType)
        {
            var vfx = Instantiate(gemPopVFX, grid.GetWorldPositionCenter(gemLocation.x, gemLocation.y), Quaternion.Euler(grid.GetForward()), transform);

            var particlesMain = vfx.GetComponent<ParticleSystem>().main;
            particlesMain.startColor = new ParticleSystem.MinMaxGradient(gemType.color);
            vfx.transform.localScale = Vector3.one * popFXScaleFactor;

        }

        private HashSet<Vector2Int> FindGroupMatches()
        {
            var neededType = grid.GetObject(selectedGem.x, selectedGem.y).GetItem().GetItemType();

            HashSet<Vector2Int> oldMatches = new();

            HashSet<Vector2Int> newlyFoundMatches = new() { selectedGem };

            HashSet<Vector2Int> batchToCheck = new();

            while (newlyFoundMatches.Count > 0)
            {
                batchToCheck.Clear();
                batchToCheck.UnionWith(newlyFoundMatches);

                foreach (var match in batchToCheck)
                {
                    // add the processed match to oldmatches
                    oldMatches.Add(match);
                    newlyFoundMatches.Remove(match);

                    // get spots on 4 sides around match
                    HashSet<Vector2Int> spotsAround = grid.GetAdjacentCoordinates(match.x, match.y);

                    // exclude the ones already in matches
                    spotsAround.ExceptWith(oldMatches);
                    if (spotsAround.Count == 0) continue;

                    // exclude the ones already in checks
                    spotsAround.ExceptWith(newlyFoundMatches);
                    if (spotsAround.Count == 0) continue;

                    // filter by being a griditem?
                    spotsAround.RemoveWhere(s => grid.GetObject(s.x, s.y).GetItem() is not GridItem);
                    if (spotsAround.Count() == 0) continue;

                    // filter by gemtype
                    spotsAround.RemoveWhere(s => grid.GetObject(s.x, s.y).GetItem().GetItemType() != neededType);
                    if (spotsAround.Count() == 0) continue;

                    // if what's left > 0, add it to newly found matches
                    newlyFoundMatches.UnionWith(spotsAround);

                }

            }

            return oldMatches;
        }

        void GridAutoCenter()
        {
            if (gridAutoCenter)
            {
                originPosition.x = -width * cellSize * 0.5f;
                originPosition.y = -height * cellSize * 0.5f;
            }
        }

        void InitializeGrid()
        {
            grid = GridSystem2D<GridObject<GridItem>>.VerticalGrid(width, height, cellSize, originPosition, debug);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (sandStonesAtTheBottom > 0)
                    {
                        CreateItem(x, y, sandStoneType, silently: true);
                        sandStonesAtTheBottom--;
                    }
                    else
                    {
                        CreateItem(x, y, silently: true);
                    }
                }
            }

        }

        private void CreateItem(int x, int y, bool silently = false)
        {
            CreateItem(x, y, gemTypes[Random.Range(0, gemTypes.Length)], silently);

        }

        // TODO: Item Pool
        private void CreateItem(int x, int y, GridItemType type, bool silently = false)
        {
            GridItem item = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            var gridObject = new GridObject<GridItem>(grid, x, y);

            item.Init(gridObject, type);
            gridObject.SetItem(item);
            grid.SetObject(x, y, gridObject);

            if (!silently) audioManager.PlayCreate();
        }
    }
}
