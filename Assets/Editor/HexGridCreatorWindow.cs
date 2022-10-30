
//using UnityEditor;
//using UnityEngine;


//namespace ProduktivEditor.HexGrid
//{
//    public class HexGridCreatorWindow : EditorWindow
//    {
//        private int dimensionWidth = 5;
//        private int dimensionHeight = 5;

//        private int paddingX = 0;
//        private int paddingZ = 0;

//        private static string hexCellPrefabDefaultPath = "Assets/_prefabs/Hex Cell.prefab";

//        private static GameObject hexCellPrefab;

//        [MenuItem("Produktivkeller/Hex Grid - Creator")]
//        public static void ShowWindow() {
//            // Try to load a default prefab. This should be adjusted after renaming the prefab!
//            hexCellPrefab = AssetDatabase.LoadAssetAtPath(hexCellPrefabDefaultPath, typeof(GameObject)) as GameObject;

//            GetWindow(typeof(HexGridCreatorWindow), true, "Hex Grid - Creator");
//        }

//        void OnGUI() {
//            GUILayout.Label("Hex Grid Creator", EditorStyles.boldLabel);
//            GUILayout.Label("The \"Hex Grid Creator\" can be used to create a new \"Hex Grid\" element in the hierarchy.\n\n" +
//                            "The new element will be placed at the root level.\n\n" +
//                            "Existing \"Hex Grid's\" will NOT get deleted.", EditorStyles.helpBox);

//            GUILayout.Label("Dimension", EditorStyles.boldLabel);
//            this.dimensionWidth = EditorGUILayout.IntSlider("Width", this.dimensionWidth, 1, 70);
//            this.dimensionHeight = EditorGUILayout.IntSlider("Height", this.dimensionHeight, 1, 70);

//            GUILayout.Label("Padding", EditorStyles.boldLabel);
//            this.paddingX = EditorGUILayout.IntSlider("Padding X", this.paddingX, 0, 20);
//            this.paddingZ = EditorGUILayout.IntSlider("Padding Z", this.paddingZ, 0, 20);

//            GUILayout.Label("Editor Configuration", EditorStyles.boldLabel);

//            // The false parameter makes sure that no scene object can be assigned. Therefore, it must be a prefab.
//            hexCellPrefab = EditorGUILayout.ObjectField("Hex Cell Prefab", hexCellPrefab, typeof(GameObject), false) as GameObject;

//            if (GUILayout.Button("Create")) {
//                Transform hexGridTransform = this.CreateGrid();
//                this.CreateMotionLayer(hexGridTransform);
//                this.Close();
//            }
//        }

//        private Transform CreateGrid() {
//            GameObject hexGrid = new GameObject("Hex Grid");
//            hexGrid.transform.SetParent(GameObject.Find("[2] Meshes").transform);

//            GameObject hexCellContainer = new GameObject("Hex Cell Container");

//            hexCellContainer.transform.SetParent(hexGrid.transform, false);

//            for (int rowIndex = 0; rowIndex < this.dimensionHeight; rowIndex++) {
//                GameObject row = this.CreateRow(hexCellContainer.transform, rowIndex);

//                for (int columnIndex = 0; columnIndex < this.dimensionWidth; columnIndex++) {
//                    this.CreateCell(row.transform, columnIndex, rowIndex);
//                }
//            }

//            return hexGrid.transform;
//        }

//        private GameObject CreateRow(Transform parent, int index) {
//            GameObject row = new GameObject("Row - " + index);
//            row.transform.parent = parent;
//            return row;
//        }

//        private void CreateCell(Transform parent, int column, int row) {
//            Vector3 position = this.CalculateLocalPositionForCell(column, row);

//            Transform cellTransform = this.CreateCellFromPrefab(column, row);

//            this.SetParentAndApplyLocalPosition(cellTransform, parent, position);
//        }

//        /*
//         * This part defines where each cell will be placed relative to its parent.
//         * 
//         * This formula was created in three steps:
//         * 
//         * 1. column * diameter                            - This creates a simple chess field. That's why we need to move every new row half a radius to the side. Results in:
//         * 2. (column + row * 0.5f) * diameter             - This creates a hexagonal field but each row moves further to the side resulting in a diagonal field. To fix this:
//         * 3. (column + row * 0.5f - row / 2) * diameter   - This works as expected. Note that row / 2 is an operator on integers returning an integer that increments half as fast as row does.
//         *                                                   That means every second row will be moved on diameter to the (-)side.
//         */
//        private Vector3 CalculateLocalPositionForCell(int column, int row) {
//            Vector3 position;

//            position.x = (column + row * 0.5f - row / 2) * (HexMetrics.innerRadius * 2);
//            position.y = 0f;
//            position.z = row * (HexMetrics.outerRadius * 1.5f);

//            // Sets the padding for a cell:
//            position.x += column * this.paddingX;
//            position.z += row * this.paddingZ;

//            return position;
//        }

//        private Transform CreateCellFromPrefab(int column, int row) {
//            GameObject hexCell = Instantiate<GameObject>(hexCellPrefab);

//            Field field = hexCell.GetComponent<Field>();
//            field.Coordinates = Coordinates.From2DArray(column, row);

//            hexCell.name = "Hex Cell - " + field.Coordinates.ToShortString();

//            return hexCell.transform;
//        }

//        private void SetParentAndApplyLocalPosition(Transform cell, Transform parent, Vector3 localPosition) {
//            cell.transform.SetParent(parent, false);
//            cell.transform.localPosition = localPosition;
//        }

//        private void CreateMotionLayer(Transform parent) {
//            GameObject motionLayer = GameObject.CreatePrimitive(PrimitiveType.Plane);
//            motionLayer.name = "Motion Layer";
//            motionLayer.AddComponent(typeof(IndicatorMotionLayer));
//            motionLayer.GetComponent<MeshRenderer>().enabled = false;
//            motionLayer.SetLayer(Layer.MotionLayer);

//            motionLayer.transform.SetParent(parent, false);
//            motionLayer.transform.localScale = new Vector3(500, 1, 500);
//            motionLayer.transform.position = Misc.UNIT_LOCALPOSITION;
//        }
//    }
//}
