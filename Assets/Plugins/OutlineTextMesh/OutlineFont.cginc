// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
};

struct v2f {
	float4 vertex : SV_POSITION;
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
};

sampler2D _MainTex;
uniform float4 _MainTex_ST;
uniform fixed4 _Color;
uniform fixed4 _OutlineColor;
uniform float _OuterThickness;
uniform float4 _InnerThickness;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.color = v.color * _Color;
	o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
	return o;
}

v2f vert_o (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.color = _OutlineColor;
	o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
	float rw = 1.0 / o.vertex.w;
	o.vertex.xy += _OuterThickness * float2(OUTLINE_THICKNESS_X, OUTLINE_THICKNESS_Y) / rw;
	return o;
}

inline fixed _GetInnerA (float2 uv, float scl)
{
	float2 xy = _InnerThickness.xy * scl;
	float2 xy2 = xy * (1.0 / 1.41421356);

	fixed texA0 = tex2D(_MainTex, float2(uv.x - xy.x, uv.y)).a;
	fixed texA1 = tex2D(_MainTex, float2(uv.x + xy.x, uv.y)).a;
	fixed texA2 = tex2D(_MainTex, float2(uv.x, uv.y - xy.y)).a;
	fixed texA3 = tex2D(_MainTex, float2(uv.x, uv.y + xy.y)).a;

	fixed texA4 = tex2D(_MainTex, float2(uv.x - xy2.x, uv.y - xy2.y)).a;
	fixed texA5 = tex2D(_MainTex, float2(uv.x + xy2.x, uv.y - xy2.y)).a;
	fixed texA6 = tex2D(_MainTex, float2(uv.x - xy2.x, uv.y + xy2.y)).a;
	fixed texA7 = tex2D(_MainTex, float2(uv.x + xy2.x, uv.y + xy2.y)).a;
	
	fixed r = min(texA0,texA1);
	r = min(r,texA2);
	r = min(r,texA3);
	r = min(r,texA4);
	r = min(r,texA5);
	r = min(r,texA6);
	r = min(r,texA7);
	return r;
}

fixed4 frag_o (v2f i) : SV_Target
{
	fixed4 col = i.color;
	
	fixed texA = tex2D(_MainTex, i.texcoord).a;
	col.a *= texA;
	return col;
}

fixed4 frag (v2f i) : SV_Target
{
	fixed4 col = i.color;
	
	fixed innerA0 = _GetInnerA(i.texcoord, 1.0);
	fixed innerA1 = _GetInnerA(i.texcoord, 0.5);
	
	fixed texA = tex2D(_MainTex, i.texcoord).a;
	col.a *= texA;

	fixed originalA = texA * min(innerA0, innerA1);
	col.rgb = col.rgb * originalA + _OutlineColor.rgb * (1.0 - originalA);
	return col;
}
