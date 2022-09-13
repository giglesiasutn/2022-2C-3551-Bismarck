using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bismarck
{
     class MatrixModel
    {
        public Matrix Matrix { get; set; }
        public Model Model { get; set; }

        public MatrixModel(Matrix matrix, Model model)
        {
            Matrix = matrix;
            Model = model;
        }

    }
}
