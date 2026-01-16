Shader "Custom/Bumped Specular Ambient Occlusion" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_SpecColor ("Specular", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.08
		_MainTex ("Base(RGB) Alpha(A)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_AOTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
		_AOFac ("Ambient Occlusion factor", Range (0, 1)) = 1
		_Cutoff ("Cutoff", float) = 0.5
	}
	SubShader { 
		Tags { "RenderType"="TransparentCutout" }
		LOD 200

		CGPROGRAM
		#pragma surface surf BlinnPhong alphatest:_Cutout addshadow
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _AOTex;
		float4 _Color;
		float _Shininess;
		half _AOFac;
		fixed _Cutoff;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv2_AOTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
		
			half4 tex = tex2D(_MainTex, IN.uv_MainTex);
			half4 ao = tex2D(_AOTex, IN.uv2_AOTex);
			
			ao.rgb = ((ao.rgb - 0.5f) * max(_AOFac, 0)) + 0.5f;
			
			o.Gloss = tex.a;
			
			o.Specular = _Shininess;
			
			o.Albedo = tex.rgb * _Color.rgb * ao.rgb;
			
			o.Alpha = tex.a;
			clip (o.Alpha - _Cutoff);
			
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}

	FallBack "Legacy Shaders/Lightmapped/Bumped Specular"
}

//Thanks to Laurent Clave for this shader