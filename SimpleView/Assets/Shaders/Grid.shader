Shader "Custom/Grid" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,0)
	}

	SubShader {
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off Cull Off Fog { Mode Off }
			Color[_Color]
			BindChannels {
				Bind "vertex", vertex
				Bind "color", color
			}
		}
	}
}
