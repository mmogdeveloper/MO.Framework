// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "KriptoFX/RFX4/Particle" {
	Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
	 [HideInInspector]_Cutout ("_Cutout", Float) = 0.2
	 [HideInInspector]_InvFade ("Soft Particles Factor", Float) = 1.0
	 [HideInInspector]SrcMode ("SrcMode", int) = 1
     [HideInInspector]DstMode ("DstMode", int) = 1
	 [HideInInspector]_Tiling ("Tiling", Vector) = (800, 800, 8, 8)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "RFX4"="Particle"}
				Blend [SrcMode] [DstMode]
				Lighting On
				Cull Off 
				ZWrite Off
	SubShader {
		Pass {
				
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			#pragma multi_compile_fog
			#pragma multi_compile BlendAdd BlendAlpha BlendMul BlendMul2
			#pragma multi_compile VertLight_OFF VertLight4_ON VertLight4Normal_ON
			#pragma multi_compile FrameBlend_OFF FrameBlend_ON
			#pragma multi_compile SoftParticles_OFF SoftParticles_ON
			#pragma multi_compile Clip_OFF Clip_ON Clip_ON_Alpha
#pragma target 3.0
			
			#include "UnityCG.cginc"
			#include "RFX4_ShaderExtension.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float4 _Tiling;
			float _Cutout;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
			};

			v2f vert (appdata_t v)
			{
				v2f o;
#if UNITY_VERSION >= 550
				o.vertex = UnityObjectToClipPos(v.vertex);
#else 
				o.vertex = UnityObjectToClipPos(v.vertex);
#endif
			#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
			#endif
				o.color = v.color;
				o.color.rgb *= ComputeVertexLight(v.vertex, v.normal);

				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D _CameraDepthTexture;
			float _InvFade;
			
			half4 frag (v2f i) : SV_Target
			{
			#ifdef SoftParticles_ON
				#ifdef SOFTPARTICLES_ON
					float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
					float partZ = i.projPos.z;
					float fade = saturate (_InvFade * (sceneZ-partZ));
					i.color.a *= fade;
				#endif
			#endif
				
			#ifdef FrameBlend_OFF
				half4 tex = tex2D(_MainTex, i.texcoord);
			#else
				half4 tex = Tex2DInterpolated(_MainTex, i.texcoord, _Tiling);
			#endif

				half4 res = 2 * tex * _TintColor;

			#ifdef Clip_ON
				res.a = step(_Cutout, tex.a) * res.a;
			#endif	

			#ifdef Clip_ON_Alpha
				res.a = step(1-i.color.a + _Cutout, tex.a);
				res.rgb *= i.color.rgb;
			#endif	

			#if !defined(Clip_ON_Alpha)
				res *= i.color;
			#endif	

				//res *= i.color;
				res.a = saturate(res.a);

			#ifdef BlendAdd
				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, half4(0,0,0,0)); 
			#endif
			#ifdef BlendAlpha
				UNITY_APPLY_FOG(i.fogCoord, res);
			#endif
			#ifdef BlendMul
				res = lerp(half4(1,1,1,1), res, res.a);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, half4(1,1,1,1)); // fog towards white due to our blend mode
			#endif
			#ifdef BlendMul2
				res = lerp(half4(0.5,0.5,0.5,0.5), res, res.a);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, half4(0.5,0.5,0.5,0.5)); // fog towards gray due to our blend mode
			#endif
				return res;
			}
			ENDCG 
		}
	}	
}
 CustomEditor "RFX4_CustomMaterialInspectorParticle"
}