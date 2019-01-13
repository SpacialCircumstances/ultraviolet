#version 140

in	vec4 uv_Position0;
#ifdef HAS_NORMALS
in	vec4 uv_Normal0;
#endif
#ifdef HAS_TANGENTS
in	vec4 uv_Tangent0;
#endif
#ifdef HAS_UV
in	vec4 uv_TextureCoordinate0;
#endif

uniform mat4 World;
uniform mat4 WorldViewProjection;
uniform mat4 NormalMatrix;

out	vec3 vPosition;
out	vec3 vTextureCoordinate;
#ifdef HAS_NORMALS
#ifdef HAS_TANGENTS
out	mat3 vTangentBinormalNormal;
#else
out vec3 vNormal;
#endif
#endif

void main()
{
	vec4 pos = uv_Position0 * World;
	vPosition = vec3(pos.xyz) / pos.w;

	#ifdef HAS_NORMALS
	#ifdef HAS_TANGENTS
	vec3 normalW = normalize(vec3(vec4(uv_Normal0.xyz, 0.0) * NormalMatrix));
	vec3 tangentW = normalize(vec3(vec4(uv_Tangent0.xyz, 0.0) * World));
	vec3 bitangentW = uv_Tangent0.w * cross(normalW, tangentW);	
	vTangentBinormalNormal = mat3(tangentW, bitangentW, normalW);
	#else
	vNormal = normalize(vec3(vec4(uv_Normal0.xyz, 0.0) * World));
	#endif
	#endif

	#ifdef HAS_UV
	vTextureCoordinate = uv_TextureCoordinate0;
	#else
	vTextureCoordinate = vec2(0, 0);
	#endif

	gl_Position = uv_Position0 * WorldViewProjection;
}