using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Effects/Extensions/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        protected override void Start()
        {
            this.targetGraphic = base.GetComponent<Graphic>();
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            int currentVertCount = vh.currentVertCount;
            if (!this.IsActive() || currentVertCount == 0)
            {
                return;
            }
            List<UIVertex> list = new List<UIVertex>();
            vh.GetUIVertexStream(list);
            UIVertex uivertex = default(UIVertex);
            if (this.gradientMode == GradientMode.Global)
            {
                if (this.gradientDir == GradientDir.DiagonalLeftToRight || this.gradientDir == GradientDir.DiagonalRightToLeft)
                {
                    this.gradientDir = GradientDir.Vertical;
                }
                float num = (this.gradientDir == GradientDir.Vertical) ? list[list.Count - 1].position.y : list[list.Count - 1].position.x;
                float num2 = ((this.gradientDir == GradientDir.Vertical) ? list[0].position.y : list[0].position.x) - num;
                for (int i = 0; i < currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref uivertex, i);
                    if (this.overwriteAllColor || !(uivertex.color != this.targetGraphic.color))
                    {
                        uivertex.color *= Color.Lerp(this.vertex2, this.vertex1, (((this.gradientDir == GradientDir.Vertical) ? uivertex.position.y : uivertex.position.x) - num) / num2);
                        vh.SetUIVertex(uivertex, i);
                    }
                }
                return;
            }
            for (int j = 0; j < currentVertCount; j++)
            {
                vh.PopulateUIVertex(ref uivertex, j);
                if (this.overwriteAllColor || this.CompareCarefully(uivertex.color, this.targetGraphic.color))
                {
                    switch (this.gradientDir)
                    {
                        case GradientDir.Vertical:
                            uivertex.color *= ((j % 4 == 0 || (j - 1) % 4 == 0) ? this.vertex1 : this.vertex2);
                            break;
                        case GradientDir.Horizontal:
                            uivertex.color *= ((j % 4 == 0 || (j - 3) % 4 == 0) ? this.vertex1 : this.vertex2);
                            break;
                        case GradientDir.DiagonalLeftToRight:
                            uivertex.color *= ((j % 4 == 0) ? this.vertex1 : (((j - 2) % 4 == 0) ? this.vertex2 : Color.Lerp(this.vertex2, this.vertex1, 0.5f)));
                            break;
                        case GradientDir.DiagonalRightToLeft:
                            uivertex.color *= (((j - 1) % 4 == 0) ? this.vertex1 : (((j - 3) % 4 == 0) ? this.vertex2 : Color.Lerp(this.vertex2, this.vertex1, 0.5f)));
                            break;
                    }
                    vh.SetUIVertex(uivertex, j);
                }
            }
        }

        private bool CompareCarefully(Color col1, Color col2)
        {
            return Mathf.Abs(col1.r - col2.r) < 0.003f && Mathf.Abs(col1.g - col2.g) < 0.003f && Mathf.Abs(col1.b - col2.b) < 0.003f && Mathf.Abs(col1.a - col2.a) < 0.003f;
        }

        public void ForceUpdate()
        {

            //OnValidate();
            graphic.SetVerticesDirty();


        }

        public GradientMode gradientMode;

        public GradientDir gradientDir;

        public bool overwriteAllColor;

        public Color vertex1 = Color.white;

        public Color vertex2 = Color.black;

        private Graphic targetGraphic;
    }
}
