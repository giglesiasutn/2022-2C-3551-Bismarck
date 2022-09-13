using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bismarck.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal  del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        public const float CarMaxSpeed = 20.0f;
        public const float CarMinSpeed = -10.0f; //Negativo para que tenga reversa
        public const float CarAcceleration = 4.0f;
        public const float CargAngularSpeed = (float)Math.PI / 4;

        public const float IslandSize = 15000;
        public const float ShipSize = 5000;
        public const Int32 MapSize = 100000;
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            // Descomentar para que el juego sea pantalla completa.
            // Graphics.IsFullScreen = true;
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private Model ShipModel { get; set; }
        private BasicEffect Effect { get; set; }
        private float Rotation { get; set; }
        private Matrix ShipWorld { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }

        private Model ShipBModel { get; set; }
        private Model seaModel { get; set; }
        private Model IslandModel { get; set; }
        private Model IslandModel2 { get; set; }
        private Model IslandModel3 { get; set; }
        private FollowCamera FollowCamera { get; set; }

        private List<Matrix> IslandWords = new List<Matrix>();
        private List<Matrix> ShipsBWords = new List<Matrix>();
        private Camera TestCamera { get; set; }
        private VertexBuffer seaVertexBuffer { get; set; }
        private IndexBuffer seaVertexIndex { get; set; }


        private float CarCurrentSpeed;
        private float rotationY;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

            // Apago el backface culling.
            // Esto se hace por un problema en el diseno del modelo del logo de la materia.
            // Una vez que empiecen su juego, esto no es mas necesario y lo pueden sacar.
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            // Seria hasta aca.

            // Configuramos nuestras matrices de la escena.
            ShipWorld = Matrix.Identity;
            View = Matrix.CreateLookAt(Vector3.UnitZ * 5000, Vector3.Zero, Vector3.Up);
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 500);
            // Creo una camara para seguir a nuestro varco
            FollowCamera = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);
            var size = GraphicsDevice.Viewport.Bounds.Size;
            size.X /= 2;
            size.Y /= 2;
            TestCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 50, 1000), size);

            // Inicializo velocidad
            /*CarCurrentSpeed = 0;
            rotationY = 0.0f;
            isJumping = false;*/

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Cargo el modelo del logo.
            loadModels();

            loadIslands();

            loadShips();

            loadSea();

            ShipWorld = Matrix.CreateScale(0.05f);

            base.LoadContent();
        }
        
        private void loadModels() {
            ShipModel = Content.Load<Model>(ContentFolder3D + "Ship/Ship");
            ShipBModel = Content.Load<Model>(ContentFolder3D + "ShipB/Source/Ship");
            IslandModel = Content.Load<Model>(ContentFolder3D + "Island1/Island1");
            IslandModel2 = Content.Load<Model>(ContentFolder3D + "Island2/Island2");
            IslandModel3 = Content.Load<Model>(ContentFolder3D + "Island3/Island3");
            seaModel = Content.Load<Model>(ContentFolder3D + "Sea/sea_escenario_with_texture");
        }

        private void loadSea()
        {
            Effect = new BasicEffect(GraphicsDevice);
            Effect.VertexColorEnabled = true;
            var triangleVertices = new[]
            {
                new VertexPositionColor(new Vector3(-MapSize, -100, -MapSize), Color.Blue),
                new VertexPositionColor(new Vector3(-MapSize, -100, MapSize), Color.Blue),
                new VertexPositionColor(new Vector3(MapSize, -100, -MapSize), Color.Blue),
                new VertexPositionColor(new Vector3(MapSize, -100, MapSize), Color.Blue)
            };

            seaVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, triangleVertices.Length,
                BufferUsage.WriteOnly);
            seaVertexBuffer.SetData(triangleVertices);

            // Array of indices
            var triangleIndices = new ushort[]
            {
                0, 1, 2, 3, 1, 2
            };

            seaVertexIndex = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, triangleIndices.Length, BufferUsage.None);
            seaVertexIndex.SetData(triangleIndices);
        }

        private bool isInSquare(Vector3 center, float squareSize, float x, float z)
        {
            return center.X - squareSize <= x && center.X + squareSize >= x &&
                   center.Z - squareSize <= z && center.Z + squareSize >= z;
        }

        private void loadIslands()
        {
            // Cargo Islands en lugares aleatorios no repetidos
            Random r = new Random();
            for (int i = 0; i < 60; i++)
            {
                while (true)
                {
                    float x = r.Next(-MapSize, MapSize);
                    float y = 0;  //Fijo en Y
                    float z = r.Next(-MapSize, MapSize);
                    if (!IslandWords.Exists(island => isInSquare(island.Translation, IslandSize, x, z)) &&
                        !isInSquare(ShipWorld.Translation, ShipSize, x, z))
                    {
                        IslandWords.Add(Matrix.Identity * Matrix.CreateTranslation(new Vector3(x, y, z)));
                        break;
                    }
                }
            }
        }

        private void loadShips()
        {
            // Cargo Ships en posiciones aleatorias
            Random r = new Random();
            List<Matrix> WordsTotal = new List<Matrix>(IslandWords);

            for (int i = 0; i < 40; i++)
            {
                while (true)
                {
                    float x = r.Next(-MapSize, MapSize);
                    float y = 0;  //Fijo en Y
                    float z = r.Next(-MapSize, MapSize);
                    //Verifico aqui, con el total de vectores posiciones en el mundo (islas y luego islas y barcos)
                    if (!IslandWords.Exists(island => isInSquare(island.Translation, IslandSize, x, z)) &&
                        !ShipsBWords.Exists(ship => isInSquare(ship.Translation, ShipSize, x, z)) &&
                        !isInSquare(ShipWorld.Translation, ShipSize, x, z))
                    {
                        ShipsBWords.Add(Matrix.Identity * Matrix.CreateTranslation(new Vector3(x, y, z)));
                        WordsTotal.Add(Matrix.Identity * Matrix.CreateTranslation(new Vector3(x, y, z)));
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.
            TestCamera.Update(gameTime);
            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Salgo del juego.
                Exit();

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            var rotationMatrix = Matrix.CreateRotationY(Rotation);
            ShipWorld = Matrix.CreateScale(0.05f) * rotationMatrix ;


            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(Color.Cyan);

            ShipModel.Draw(ShipWorld, TestCamera.View, TestCamera.Projection);

            Model drawModel = IslandModel;
            int i = 0;
            foreach (Matrix IslandWord in IslandWords)
            {
                if (i == 20)
                    drawModel = IslandModel2;
                else if (i == 40)
                    drawModel = IslandModel3;

                drawModel.Draw(IslandWord * Matrix.CreateScale(0.3f), TestCamera.View, TestCamera.Projection);
                i++;
            }
            foreach (Matrix ShipsBWord in ShipsBWords)
            {
                ShipBModel.Draw(ShipsBWord * Matrix.CreateScale(0.6f), TestCamera.View, TestCamera.Projection);
            }
            
            drawSea();
        }

        private void drawSea()
        {
            GraphicsDevice.SetVertexBuffer(seaVertexBuffer);
            GraphicsDevice.Indices = seaVertexIndex;
            Effect.World = Matrix.Identity;
            Effect.View = TestCamera.View;
            Effect.Projection = TestCamera.Projection;
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }
        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }
    }
}