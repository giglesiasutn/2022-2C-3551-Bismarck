using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private Effect Effect { get; set; }
        private float Rotation { get; set; }
        private Matrix ShipWorld { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }

        private Model seaModel { get; set; }
        private FollowCamera FollowCamera { get; set; }
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
            View = Matrix.CreateLookAt(Vector3.UnitZ * 250, Vector3.Zero, Vector3.Up);
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 550);
            // Creo una camara para seguir a nuestro auto
            FollowCamera = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);

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
            ShipModel = Content.Load<Model>(ContentFolder3D + "Ship/Ship");
            seaModel = Content.Load<Model>(ContentFolder3D + "Sea/sea_escenario_with_texture");
            
            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");
            ShipWorld = Matrix.CreateScale(0.05f);

            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.

            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Salgo del juego.
                Exit();

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            var rotationMatrix = Matrix.CreateRotationY(Rotation);
            ShipWorld = Matrix.CreateScale(0.05f) * rotationMatrix;
            // Actualizo la camara, enviandole la matriz de mundo del auto
            FollowCamera.Update(gameTime, ShipWorld);

            //Movimiento
            /*
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Vector3 previousPosition = CarWorld.Translation;

            //Giros
            if (keyboardState.IsKeyDown(Keys.D))
                rotationY += CargAngularSpeed * elapsedTime * -1.0f * CarCurrentSpeed / 4;
            if (keyboardState.IsKeyDown(Keys.A))
                rotationY += CargAngularSpeed * elapsedTime * CarCurrentSpeed / 4;

            CarWorld = Matrix.CreateRotationY(rotationY);

            //Aceleracion y desaleracion (+marcha atras)
            if (keyboardState.IsKeyDown(Keys.W))
                CarCurrentSpeed += CarAcceleration * elapsedTime;
            else if (keyboardState.IsKeyDown(Keys.S))
                CarCurrentSpeed -= CarAcceleration * elapsedTime;
            else
            {
                //Para que desacelere solo al dejar de avanzar o retroceder
                if (CarCurrentSpeed > 0)
                    CarCurrentSpeed -= CarAcceleration * elapsedTime;
                else if (CarCurrentSpeed < 0)
                    CarCurrentSpeed += CarAcceleration * elapsedTime;
            }

            if (CarCurrentSpeed > CarMaxSpeed)
                CarCurrentSpeed = CarMaxSpeed;
            if (CarCurrentSpeed < CarMinSpeed)
                CarCurrentSpeed = CarMinSpeed;

            Vector3 movement = CarWorld.Forward * CarCurrentSpeed;

            ShipWorld = ShipWorld * Matrix.CreateTranslation(previousPosition + movement);
            */
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

            // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
            Effect.Parameters["View"].SetValue(FollowCamera.View);
            Effect.Parameters["Projection"].SetValue(FollowCamera.Projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Beige.ToVector3());

            ShipModel.Draw(ShipWorld, FollowCamera.View, FollowCamera.Projection);
            seaModel.Draw(Matrix.CreateScale(10), FollowCamera.View, FollowCamera.Projection);
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