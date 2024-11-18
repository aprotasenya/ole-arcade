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
            if (grid.IsEmpty(gridPosition.x, gridPosition.y)) {
                return;
            }

            if (selectedGem == gridPosition)
            {
                DeselectGem();
            }
            else if (selectedGem == Vector2Int.one * -1)
            {
                SelectGem(gridPosition);
                EnableOutline(gridPosition, true);
                audioManager.PlaySelect();
            }
            else
            {
                // TODO: Limit to only neighbour swaps?
                // TODO: Limit valid moves only to matching ones?

                EnableOutline(gridPosition, true);
                audioManager.PlaySelect();
                StartCoroutine(SwapGems(selectedGem, gridPosition));
                DeselectGem();
            }
        }

        private void EnableOutline(Vector2Int gridPosition, bool enabled)
        {
            var gemOutline = grid.GetValue(gridPosition.x, gridPosition.y)?.GetValue()?.gameObject?.GetComponent<Outline>();
            if (gemOutline != null) gemOutline.enabled = enabled;

        }

        private void SelectGem(Vector2Int gridPosition)
        {
            selectedGem = gridPosition;
        }

        private void DeselectGem()
        {
            EnableOutline(selectedGem, false);
            selectedGem = Vector2Int.one * -1;

        }

        IEnumerator RunMatchLoop()
        {
            List<Vector2Int> matches = FindMatches();

            while (matches.Count > 0)
            {
                yield return StartCoroutine(ExplodeGems(matches));

                // TODO: Calculate score

                yield return StartCoroutine(MakeGemsFall());

                yield return StartCoroutine(FillEmptySpots());

                matches = FindMatches();
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
            // TODO: Make this more efficient (could be faster & readable)

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid.GetValue(x, y) == null)
                    {
                        for (int i = y + 1; i < height; i++)
                        {
                            if (grid.GetValue(x, i) != null)
                            {
                                var gem = grid.GetValue(x, i).GetValue();
                                grid.SetValue(x, y, grid.GetValue(x, i));
                                grid.SetValue(x, i, null);
                                gem.transform.DOLocalMove(grid.GetWorldPositionCenter(x, y), gemDropTime).SetEase(gemDropEase);
                                audioManager.PlayDrop();
                                yield return new WaitForSeconds(gemDropTime * gemDropWaitFactor);
                                break;
                            }
                        }
                    }
                }
            }

        }

        private IEnumerator ExplodeGems(List<Vector2Int> matches)
        {
            foreach (var match in matches)
            {
                var gem = grid.GetValue(match.x, match.y).GetValue();
                grid.SetValue(match.x, match.y, null);

                ExplodeFX(match, gem.GetGemType());

                gem.transform.DOPunchScale(Vector3.one * 0.1f, duration: 0.1f, vibrato: 1, elasticity: 0.5f);

                yield return new WaitForSeconds(0.1f);

                gem.DestroyGem();
            }
        }

        private void ExplodeFX(Vector2Int gemLocation, GemType gemType)
        {
            // TODO: FX Pool
            var vfx = Instantiate(gemPopVFX, grid.GetWorldPositionCenter(gemLocation.x, gemLocation.y), Quaternion.Euler(grid.GetForward()), transform);

            // TODO: Fix this deprecated call
            vfx.GetComponent<ParticleSystem>().startColor = gemType.color;
            vfx.transform.localScale = Vector3.one * popFXScaleFactor;

            audioManager.PlayPop();
        }

        private List<Vector2Int> FindMatches()
        {
            HashSet<Vector2Int> matches = new();

            // Horizontal
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width - 2; x++)
                {
                    var gemA = grid.GetValue(x, y);
                    var gemB = grid.GetValue(x + 1, y);
                    var gemC = grid.GetValue(x + 2, y);

                    if (gemA == null || gemB == null | gemC == null) continue;

                    if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType()
                        && gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x + 1, y));
                        matches.Add(new Vector2Int(x + 2, y));

                    }
                }
            }

            // Vertical
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height - 2; y++)
                {
                    var gemA = grid.GetValue(x, y);
                    var gemB = grid.GetValue(x, y + 1);
                    var gemC = grid.GetValue(x, y + 2);

                    if (gemA == null || gemB == null | gemC == null) continue;

                    if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType()
                        && gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x, y + 1));
                        matches.Add(new Vector2Int(x, y + 2));

                    }
                }
            }

            if (matches.Count == 0)
            {
                audioManager.PlayNoMatch();
            } else
            {
                audioManager.PlayMatch();
            }

            return new List<Vector2Int>(matches);

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

            EnableOutline(gridPositionA, false);
            EnableOutline(gridPositionB, false);

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

            StartCoroutine(RunMatchLoop());
        }

        private void CreateGem(int x, int y, bool silently = false)
        {
            if (!silently) audioManager.PlayCreate();

            Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            gem.SetType(gemTypes[Random.Range(0, gemTypes.Length)]);

            var gridObject = new GridObject<Gem>(grid, x, y);
            gridObject.SetValue(gem);
            grid.SetValue(x, y, gridObject);
        }
    }
}
