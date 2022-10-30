using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProduktivEditor.DiamondSquare
{
    public class DiamondSquareAlgorithm
    {
        private List<List<UnityEngine.Transform>> fields;
        private uint fieldSize;
        private float initialDiamondSquareFactor;

        public DiamondSquareAlgorithm(List<List<UnityEngine.Transform>> fields, float diamondSquareFactor) {
            this.fields = fields;
            this.fieldSize = (uint)this.fields[0].Count;
            this.initialDiamondSquareFactor = diamondSquareFactor;
        }

        public void ExecuteAlgorithm() {
            this.ExecuteAlgorithm(TwoDCoordinate.Zero(), this.fieldSize - 1, this.initialDiamondSquareFactor);
        }

        private void ExecuteAlgorithm(TwoDCoordinate lowerLeftCoordinate, uint size, float diamondSquareFactor) {
            this.ApplyDiamondStep(lowerLeftCoordinate, size, diamondSquareFactor);
            this.ApplySquareStep(lowerLeftCoordinate, size, diamondSquareFactor);

            if (size > 2) {
                uint newSize = (uint)(size / 2f);
                float newDiamondSquareFactor = diamondSquareFactor / 2.0f;

                this.ExecuteAlgorithm(lowerLeftCoordinate, newSize, newDiamondSquareFactor);
                this.ExecuteAlgorithm(lowerLeftCoordinate.Right(newSize), newSize, newDiamondSquareFactor);
                this.ExecuteAlgorithm(lowerLeftCoordinate.Top(newSize), newSize, newDiamondSquareFactor);
                this.ExecuteAlgorithm(lowerLeftCoordinate.Top(newSize).Right(newSize), newSize, newDiamondSquareFactor);
            }
        }

        private void ApplyDiamondStep(TwoDCoordinate lowerLeftCoordinate, uint size, float diamondSquareFactor) {
            TwoDCoordinate centerCoordinate = lowerLeftCoordinate.Top(size / 2).Right(size / 2);

            if (centerCoordinate.Row % 2 == 0) {
                float centerHeight = this.CalculateCenterHeight(lowerLeftCoordinate, size);
                centerHeight += this.DiamondSquareRandomization(diamondSquareFactor);
                this.ApplyHeight(centerCoordinate, centerHeight);

                return;
            }


            TwoDCoordinate rightCenterCoordinate = centerCoordinate;
            TwoDCoordinate leftCenterCoordinate = rightCenterCoordinate.Left(1);

            float[] centerHeights = this.CalculateCenterHeights(lowerLeftCoordinate, size);

            float leftCenterHeight = centerHeights[0] + this.DiamondSquareRandomization(diamondSquareFactor);
            float rightCenterHeight = centerHeights[1] + this.DiamondSquareRandomization(diamondSquareFactor);

            this.ApplyHeight(leftCenterCoordinate, leftCenterHeight);
            this.ApplyHeight(rightCenterCoordinate, rightCenterHeight);
        }

        private float CalculateCenterHeight(TwoDCoordinate lowerLeftCoordinate, uint size) {
            float height = this.ReadHeight(lowerLeftCoordinate);
            height += this.ReadHeight(lowerLeftCoordinate.Right(size));
            height += this.ReadHeight(lowerLeftCoordinate.Top(size));
            height += this.ReadHeight(lowerLeftCoordinate.Top(size).Right(size));

            return height / 4f;
        }

        private float[] CalculateCenterHeights(TwoDCoordinate lowerLeftCoordinate, uint size) {
            float lowerLeftHeight = this.ReadHeight(lowerLeftCoordinate);
            float lowerRightHeight = this.ReadHeight(lowerLeftCoordinate.Right(size));
            float upperLeftHeight = this.ReadHeight(lowerLeftCoordinate.Top(size));
            float upperRightHeight = this.ReadHeight(lowerLeftCoordinate.Top(size).Right(size));

            float leftCenterHeight = 2 * lowerLeftHeight + 2 * upperLeftHeight + lowerRightHeight + upperRightHeight;
            leftCenterHeight /= 6.0f;

            float rightCenterHeight = lowerLeftHeight + upperLeftHeight + 2 * lowerRightHeight + 2 * upperRightHeight;
            rightCenterHeight /= 6.0f;

            return new float[] { leftCenterHeight, rightCenterHeight };
        }

        private float ReadHeight(TwoDCoordinate twoDCoordinate) {
            return this.fields[(int)twoDCoordinate.Row][(int)twoDCoordinate.Column].position.y;
        }

        private void ApplyHeight(TwoDCoordinate twoDCoordinate, float height) {
            UnityEngine.Transform field = this.fields[(int)twoDCoordinate.Row][(int)twoDCoordinate.Column];
            Vector3 position = field.position;
            position.y = height;
            field.position = position;
        }

        private void ApplySquareStep(TwoDCoordinate lowerLeftCoordinate, uint size, float diamondSquareFactor) {
            uint sizeHalf = size / 2;

            TwoDCoordinate topCoordinate = lowerLeftCoordinate.Right(sizeHalf).Top(size);
            TwoDCoordinate bottomCoordinate = lowerLeftCoordinate.Right(sizeHalf);

            if (size > 2) {
                TwoDCoordinate leftCoordinate = lowerLeftCoordinate.Top(sizeHalf);
                TwoDCoordinate rightCoordinate = lowerLeftCoordinate.Top(sizeHalf).Right(size);

                this.ApplyHeightForSquareStepCoordinate(leftCoordinate, sizeHalf, diamondSquareFactor);
                this.ApplyHeightForSquareStepCoordinate(rightCoordinate, sizeHalf, diamondSquareFactor);
            }

            this.ApplyHeightForSquareStepCoordinate(topCoordinate, sizeHalf, diamondSquareFactor);
            this.ApplyHeightForSquareStepCoordinate(bottomCoordinate, sizeHalf, diamondSquareFactor);
        }

        private void ApplyHeightForSquareStepCoordinate(TwoDCoordinate coordinate, uint size, float diamondSquareFactor) {
            List<float> neighbourHeights = new List<float>();

            TwoDCoordinate[] neighbourCoordinates = coordinate.LeftTopRightBottom(size);

            foreach (TwoDCoordinate neighbourCoordinate in neighbourCoordinates) {
                if (this.CheckIfCoordinateIsInsideField(neighbourCoordinate)) {
                    neighbourHeights.Add(this.ReadHeight(neighbourCoordinate));
                }
            }

            float height = neighbourHeights.Sum() / neighbourHeights.Count;
            height += this.DiamondSquareRandomization(diamondSquareFactor);

            this.ApplyHeight(coordinate, height);
        }

        private float DiamondSquareRandomization(float diamondSquareFactor) {
            return Random.Range(-1f, 1f) * diamondSquareFactor;
        }

        private bool CheckIfCoordinateIsInsideField(TwoDCoordinate coordinate) {
            bool rowIsInsideField = coordinate.Row >= 0 && coordinate.Row < this.fieldSize;
            bool columnIsInsideField = coordinate.Column >= 0 && coordinate.Column < this.fieldSize;

            return rowIsInsideField && columnIsInsideField;
        }
    }
}

    class TwoDCoordinate
    {
        public uint Row;
        public uint Column;

        public static TwoDCoordinate Zero() {
            return new TwoDCoordinate {
                Row = 0,
                Column = 0
            };
        }

        public TwoDCoordinate Right(uint offset) {
            return new TwoDCoordinate {
                Row = this.Row,
                Column = this.Column + offset
            };
        }

        public TwoDCoordinate Top(uint offset) {
            return new TwoDCoordinate {
                Row = this.Row + offset,
                Column = this.Column
            };
        }

        public TwoDCoordinate Bottom(uint offset) {
            return new TwoDCoordinate {
                Row = this.Row - offset,
                Column = this.Column
            };
        }

        public TwoDCoordinate Left(uint offset) {
            return new TwoDCoordinate {
                Row = this.Row,
                Column = this.Column - offset
            };
        }

        public TwoDCoordinate[] LeftTopRightBottom(uint offset) {
            return new TwoDCoordinate[]
            {
                this.Left(offset),
                this.Top(offset),
                this.Right(offset),
                this.Bottom(offset),
            };
        }

        public override string ToString() {
            return string.Format("[Row={0}, Column={1}]", this.Row, this.Column);
        }
    }


