using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Match3
{
    public class Match3 : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] int width = 8;
        [SerializeField] int height = 8;
        [SerializeField] float cellSize = 1f;
        [SerializeField] Vector3 originPosition = Vector3.zero;
        [SerializeField] bool gridAutoCenter = false;
        [SerializeField] bool debug = true;

        [Header("Movement Settings")]
        [SerializeField] float gemMoveTime = 0.5f;
        [SerializeField] Ease ease = Ease.InQuad;

        [Header("Gems Settings")]
        [SerializeField] Gem gemPrefab;
        [SerializeField] GemType[] gemTypes;

        private GridSystem2D<GridObject<Gem>> grid;
        private InputReader inputReader;
        private Vector2Int selectedGem = Vector2Int.one * -1;


        private void Awake()
        {
            inputReader = GetComponent<InputReader>();
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
            }
            else
            {
                StartCoroutine(RunGameLoop(selectedGem, gridPosition));
            }
        }

        private void SelectGem(Vector2Int gridPosition) => selectedGem = gridPosition;

        private void DeselectGem() => selectedGem = Vector2Int.one * -1;

        IEnumerator RunGameLoop(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            yield return StartCoroutine(SwapGems(gridPositionA, gridPositionB));

            // TODO: Check for matches
            // TODO: Make Gems Explode
            // TODO: Fill empty spots

            DeselectGem();

            // TODO: Check for GameOver

            yield return null;
        }

        IEnumerator SwapGems(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            var gridObjectA = grid.GetValue(gridPositionA.x, gridPositionA.y);
            var gridObjectB = grid.GetValue(gridPositionB.x, gridPositionB.y);

            gridObjectA.GetValue().transform
                .DOLocalMove(grid.GetWorldPositionCenter(gridPositionB.x, gridPositionB.y), duration: gemMoveTime)
                .SetEase(ease);
            gridObjectB.GetValue().transform
                .DOLocalMove(grid.GetWorldPositionCenter(gridPositionA.x, gridPositionA.y), duration: gemMoveTime)
                .SetEase(ease);

            grid.SetValue(gridPositionA.x, gridPositionA.y, gridObjectB);
            grid.SetValue(gridPositionB.x, gridPositionB.y, gridObjectA);

            yield return new WaitForSeconds(gemMoveTime);
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
                    CreateGem(x, y);
                }
            }
        }

        private void CreateGem(int x, int y)
        {
            Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            gem.SetType(gemTypes[Random.Range(0, gemTypes.Length)]);
            var gridObject = new GridObject<Gem>(grid, x, y);
            gridObject.SetValue(gem);
            grid.SetValue(x, y, gridObject);
        }
    }
}
