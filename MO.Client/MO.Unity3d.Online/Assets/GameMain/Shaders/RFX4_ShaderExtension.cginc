
inline float4 Tex2DInterpolated(sampler2D Tex, float2 TexCoord, float4 _Tiling)
{
	float2 grid = (TexCoord * _Tiling.xy - float2(0, _Tiling.y/_Tiling.w)); 
	float2 gridFloor = floor(grid); 
				
	float frameWithLerp = (((gridFloor.x +(_Tiling.x * (_Tiling.y - gridFloor.y)) / (_Tiling.x * _Tiling.y)) * (_Tiling.z * _Tiling.w))); 
	float frame = floor(frameWithLerp);
	float lerpVal = ceil(frameWithLerp);
	
	float2 prefOffset;
	float texCell = floor(_Tiling.z);
	prefOffset.x = ((float((float(frame) % float(texCell)))) / _Tiling.z);
	prefOffset.y = ((_Tiling.w - floor(float (frame / texCell))) / _Tiling.w);

	float2 nextOffset;
	nextOffset.x = ((float((float(lerpVal) % float(texCell)))) / _Tiling.z);
	nextOffset.y = ((_Tiling.w - floor(float(lerpVal / texCell))) / _Tiling.w);
	float2 tiling = ((grid - gridFloor) / _Tiling.zw);

	float d = 1;
	float2 edge = 2.0 / _Tiling.xy;
	d *= step(edge.x, tiling.x);
	d *= step(tiling.x, 1.0/_Tiling.z - edge.x);
	d *= step(edge.y, tiling.y);
	d *= step(tiling.y, 1.0/_Tiling.w - edge.y);
	float4 tex1 = tex2D (Tex, tiling + prefOffset);
	float4 tex2 = tex2D (Tex, tiling + nextOffset);
	return lerp (tex1, tex2, frameWithLerp - frame) * d;
} 


half3 VertexLight4 (float4 vertex, float3 normal)
{
#if UNITY_VERSION >= 550
		float3 viewpos = UnityObjectToViewPos(vertex).xyz;
#else 
		float3 viewpos = mul(UNITY_MATRIX_MV, vertex).xyz;
#endif
	half3 viewN = normalize(mul ((float3x3)UNITY_MATRIX_IT_MV, normal));
	half3 light = 0;
	//[unroll(4)]
    for (int i = 0; i < 4; i++) {
		float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
		half lengthSq = dot(toLight, toLight);
		half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
		half3 color = unity_LightColor[i].rgb * atten;
		half diff = max (0, dot(viewN, normalize(toLight)));
		light += color * diff;
    }
	return light + unity_AmbientSky + unity_AmbientEquator / 10 + unity_AmbientGround / 10;
}

float3 VertexLight4 (float4 vertex)
{
#if UNITY_VERSION >= 550
	float3 viewpos = UnityObjectToViewPos(vertex).xyz;
#else 
	float3 viewpos = mul(UNITY_MATRIX_MV, vertex).xyz;
#endif
	float3 light = 0;
	//[unroll(4)]
	for (int i = 0; i < 4; i++) {
		float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
		float lengthSq = dot(toLight, toLight);
		float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
		light += unity_LightColor[i].rgb * atten;
	}
	return light + unity_AmbientSky + unity_AmbientEquator / 10 + unity_AmbientGround / 10;
}

float3 ComputeVertexLight(float4 vert, float4 norm)
{
	float3 light = 1;
	#ifdef VertLight4_ON
		light = VertexLight4(vert);
	#endif
	#ifdef VertLight4Normal_ON
		light = VertexLight4(vert, norm);
	#endif
	return light;
}
