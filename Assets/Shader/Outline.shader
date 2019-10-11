Shader "N3K/Outline"
{
    Properties
    {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)			// 메인 컬러값    
		_OutlineColor("Outline color", Color) = (1,0,0,1)		// 외곽선 색상값
		_OutlineWidth("Outline width", Range(1.0, 5.0)) = 1.1	// 외곽선 굵기
    }
	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : POSITION;
		float4 color : COLOR;
		float3 normal : NORMAL;
	};

	float _OutlineWidth;
	float4 _OutlineColor;	

	v2f vert(appdata v)
	{
		v.vertex.xyz *= _OutlineWidth;			// 버텍스 좌표를 외곽선 굵기만큼 곱해서 키워줌
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);	// 곱해준 버텍스 좌표를 카메라 클립공간으로 변환하여 저장
		o.color = _OutlineColor;				// 해당 버텍스의 컬러를 설정한 외곽선 색상으로 정함
		return o;								// 그 데이터를 리턴
	}


	ENDCG
		
    SubShader
    {
		Tags{"Queue" = "Transparent"}
         Pass // 외곽선 렌더링
		{
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag						

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}
        
		Pass // Normal render
		{
			ZWrite On	// Z버퍼 저장 -> 불투명 객체이기 때문
			
			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}
			
			Lighting On

			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
			}
			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE	// 메인 텍스처와 외곽선을 결합하여 외곽선이 그려진것처럼 보이게 구현
			}
			
		}
    }
}
