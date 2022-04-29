using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FlickerTest
{
	public class ShapeDrawer
	{
		private static Matrix Shared2DMatrix;
		private static Effect SharedEffect;
		private static bool Initialized = false;
		readonly GraphicsDevice Device;
		readonly VertexBuffer Buffer;
		int Tris = 0;
		/// <summary>
		/// Draw some goddamn shapes.
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="gfx"></param>
		/// <param name="window"></param>
		public ShapeDrawer(Effect effect, GraphicsDeviceManager gfx, GameWindow window)
		{
			if (!Initialized)
			{
				Initialized = true;
				if (gfx.GraphicsProfile == GraphicsProfile.Reach)
				{
					throw new NotSupportedException("Reach graphics profile not currently supported. Please use HiDef.");
				}
				SharedEffect ??= effect;
				gfx.PreparingDeviceSettings += (sender, e) =>
				{
					int width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth;
					int height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight;
					SetShared2DMatrix(width, height);
				};
				window.ClientSizeChanged += (sender, e) =>
				{// ClientSizeChanged is only necessary in DesktopGL, as in WinDX PreparingDeviceSettings is raised when changing window size and is therefore enough.
					SetShared2DMatrix(window.ClientBounds.Width, window.ClientBounds.Height);
				};
				// Set the shared 2D matrix immediately.
				SetShared2DMatrix(gfx.PreferredBackBufferWidth, gfx.PreferredBackBufferHeight);
			}
			Device = gfx.GraphicsDevice;
			Buffer = new DynamicVertexBuffer(Device, typeof(VertexPosition), 1200, BufferUsage.WriteOnly);
		}
		/// <summary>
		/// Set <c>Shared2DMatrix</c> so that drawing will be in screen coordinates.
		/// </summary>
		/// <param name="width">Screen width</param>
		/// <param name="height">Screen height</param>
		private static void SetShared2DMatrix(int width, int height)
		{
			Vector3 camTarget = new Vector3(0, 0, 100f);
			Vector3 camPosition = new Vector3(0, 0, 0f);
			Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, width, -height, 0, 0, 200);
			Matrix viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
			Matrix worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Down);
			Shared2DMatrix = worldMatrix * viewMatrix * projectionMatrix;
		}
		public void Begin()
		{
			Tris = 0;
		}
		public void End()
		{
			if (Tris <= 0)
			{
				return;
			}
			SharedEffect.Parameters["WorldViewProjection"].SetValue(Shared2DMatrix);
			Device.BlendState = BlendState.AlphaBlend;
			Device.DepthStencilState = DepthStencilState.None;
			Device.RasterizerState = RasterizerState.CullNone;
			SharedEffect.CurrentTechnique = SharedEffect.Techniques["BasicColorDrawing"];
			SharedEffect.CurrentTechnique.Passes[0].Apply();
			Device.SetVertexBuffer(Buffer);
			Device.DrawPrimitives(PrimitiveType.TriangleList, 0, Tris);
		}
		public void Draw(Vector2 position, float radius)
		{
			const float depth = 1.0f;
			Buffer.SetData(
				VertexPosition.VertexDeclaration.VertexStride * 3 * Tris,
				new VertexPosition[]
				{
					new VertexPosition(new Vector3(position.X - radius, position.Y - radius, depth)),
					new VertexPosition(new Vector3(position.X + radius, position.Y - radius, depth)),
					new VertexPosition(new Vector3(position.X - radius, position.Y + radius, depth)),
					new VertexPosition(new Vector3(position.X - radius, position.Y + radius, depth)),
					new VertexPosition(new Vector3(position.X + radius, position.Y - radius, depth)),
					new VertexPosition(new Vector3(position.X + radius, position.Y + radius, depth)),
				},
				0,
				6,
				VertexPosition.VertexDeclaration.VertexStride);
			Tris += 2;
		}
		public void DrawTest()
		{
			float depth = 1;
			VertexPositionColor[] vertexData = new VertexPositionColor[] {
				new VertexPositionColor(new Vector3(0,0,depth), Color.Red),
				new VertexPositionColor(new Vector3(1280,0,depth), Color.Green),
				new VertexPositionColor(new Vector3(0,720,depth), Color.Blue),
			};
			SharedEffect.Parameters["WorldViewProjection"].SetValue(Shared2DMatrix);
			Device.BlendState = BlendState.AlphaBlend;
			Device.DepthStencilState = DepthStencilState.None;
			Device.RasterizerState = RasterizerState.CullNone;
			SharedEffect.CurrentTechnique = SharedEffect.Techniques[0];
			SharedEffect.CurrentTechnique.Passes[0].Apply();

			Device.DrawUserPrimitives(PrimitiveType.TriangleList, vertexData, 0, 1);
		}
	}
}
