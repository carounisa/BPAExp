Shader "Custom/ParaboloidFragScreenSizeShader"
{
	/*
	This shader renders the given vertices as circles with the given color.
	The point size is the radius of the circle given in pixel
	Implemented using geometry shader
	*/
	Properties{
		_PointSize("Point Size", Float) = 5
		_ScreenWidth("Screen Width", Int) = 0
		_ScreenHeight("Screen Height", Int) = 0
		[Toggle] _Circles("Circles", Int) = 0
		[Toggle] _Cones("Cones", Int) = 0
		_highlight1Position("Highlight 1 Position", Vector) = (0,0,0,0)
		_highlight1Size("Highlight 1 Size", Vector) = (0,0,0,0)
	}

		SubShader
		{
			LOD 200

			Pass
			{
				Cull off

				CGPROGRAM
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag

				struct VertexInput
				{
					float4 position : POSITION;
					float4 color : COLOR;
				};

				struct VertexMiddle {
					float4 position : SV_POSITION;
					float4 size : POINTSIZE;
					float4 color : COLOR;
				};

				struct VertexOutput
				{
					float4 position : SV_POSITION;
					float4 viewposition: TEXCOORD1;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
					float size : POINTSIZE;
				};

				struct FragmentOutput
				{
					float4 color : COLOR;
					float depth : SV_DEPTH;
				};

				float _PointSize;
				int _ScreenWidth;
				int _ScreenHeight;
				int _Circles;
				int _Cones;
				float _FOV;
				float4x4 _InverseProjMatrix;

				float _hilightOpacity;

				float4 _highlight1Position;
				float4 _highlight1Size;
								
				float4 _highlight2Position;
				float4 _highlight2Size;
				
				VertexMiddle vert(VertexInput v) {
					VertexMiddle o;
					float4 viewpos = mul(UNITY_MATRIX_MV, v.position);
					o.position = mul(UNITY_MATRIX_P, viewpos);
					float slope = tan(_FOV / 2);
					o.size = -_PointSize * slope * viewpos.z * 2 / _ScreenHeight;
					o.color = v.color;

					float4 w = mul(UNITY_MATRIX_M, v.position);
					
					float dx1 = abs(_highlight1Position.x - w.x);
					float dy1 = abs(_highlight1Position.y - w.y);
					float dz1 = abs(_highlight1Position.z - w.z);

					float dx2 = abs(_highlight2Position.x - w.x);
					float dy2 = abs(_highlight2Position.y - w.y);
					float dz2 = abs(_highlight2Position.z - w.z);


					if (dx1 < _highlight1Size.x / 2 && dy1 < _highlight1Size.y / 2 && dz1 < _highlight1Size.z / 2)
					{
						// pass. point is in the first transform position
					}
					else if (dx2 < _highlight2Size.x / 2 && dy2 < _highlight2Size.y / 2 && dz2 < _highlight2Size.z / 2)
					{
						// pass. point is in the second transform position
					}
					else
					{
						// otherwise, dim the point
						o.color = v.color * _hilightOpacity;
					}
					return o;
				}

				[maxvertexcount(4)]
				void geom(point VertexMiddle input[1], inout TriangleStream<VertexOutput> outputStream) {
					float xsize = _PointSize / _ScreenWidth;
					float ysize = _PointSize / _ScreenHeight;
					VertexOutput out1;
					out1.position = input[0].position;
					out1.color = input[0].color;
					out1.uv = float2(-1.0f, 1.0f);
					out1.position.x -= out1.position.w * xsize;
					out1.position.y += out1.position.w * ysize;
					out1.position = out1.position / out1.position.w;
					out1.viewposition = mul(_InverseProjMatrix, out1.position);
					out1.viewposition /= out1.viewposition.w;
					out1.size = input[0].size;
					VertexOutput out2;
					out2.position = input[0].position;
					out2.color = input[0].color;
					out2.uv = float2(1.0f, 1.0f);
					out2.position.x += out2.position.w * xsize;
					out2.position.y += out2.position.w * ysize;
					out2.position = out2.position / out2.position.w;
					out2.viewposition = mul(_InverseProjMatrix, out2.position);
					out2.viewposition /= out2.viewposition.w;
					out2.size = input[0].size;
					VertexOutput out3;
					out3.position = input[0].position;
					out3.color = input[0].color;
					out3.uv = float2(1.0f, -1.0f);
					out3.position.x += out3.position.w * xsize;
					out3.position.y -= out3.position.w * ysize;
					out3.position = out3.position / out3.position.w;
					out3.viewposition = mul(_InverseProjMatrix, out3.position);
					out3.viewposition /= out3.viewposition.w;
					out3.size = input[0].size;
					VertexOutput out4;
					out4.position = input[0].position;
					out4.color = input[0].color;
					out4.uv = float2(-1.0f, -1.0f);
					out4.position.x -= out4.position.w * xsize;
					out4.position.y -= out4.position.w * ysize;
					out4.position = out4.position / out4.position.w;
					out4.viewposition = mul(_InverseProjMatrix, out4.position);
					out4.viewposition /= out4.viewposition.w;
					out4.size = input[0].size;
					outputStream.Append(out1);
					outputStream.Append(out2);
					outputStream.Append(out4);
					outputStream.Append(out3);
				}

				FragmentOutput frag(VertexOutput o)  {
					FragmentOutput fragout;
					float uvlen = o.uv.x*o.uv.x + o.uv.y*o.uv.y;
					if (_Circles >= 0.5 && uvlen > 1) {
						discard;
					}
					if (_Cones < 0.5) {
						o.viewposition.z += (1 - uvlen) * o.size;
					}
					else {
						o.viewposition.z += (1 - sqrt(uvlen)) * o.size;
					}
					float4 pos = mul(UNITY_MATRIX_P, o.viewposition);
					pos /= pos.w;
					fragout.depth = pos.z;
					fragout.color = o.color;
					return fragout;
				}

			ENDCG
		}
	}
}
