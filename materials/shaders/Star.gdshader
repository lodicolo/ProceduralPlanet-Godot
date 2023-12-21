shader_type spatial;
render_mode unshaded, cull_front, depth_draw_never, depth_prepass_alpha;

#include "res://materials/shaders/include/Math.gdshaderinc"

const vec2 VERTICES[3] = vec2[3] (
    vec2(-1.7321, -1.0),
    vec2(0.0, 2.0),
    vec2(1.7321, -1.0)
);

//const vec2 VERTICES[3] = vec2[3] (
    //vec2(0.0, 0.0),
    //vec2(0.5, 0.866025403785),
    //vec2(1.0, 0.0)
//);

uniform sampler2D MainTex;
uniform float daytimeFade;
uniform sampler2D Spectrum;
uniform sampler2D ClusterSpectrum;
uniform float OceanRadius;
uniform vec3 PlanetCentre;

varying vec4 col;
varying float brightnessFalloff;
varying vec3 vScreen;
varying vec3 vPos;
varying vec2 vertexOnPlane;

vec4 computeScreenPos(vec4 pos) {
	vec4 o = pos * 0.5f;
	o.xy = vec2(o.x, o.y) + o.w;
	o.zw = pos.zw;
	return o;
}

void vertex() {
	int offset = VERTEX_ID % 3;
	vertexOnPlane = VERTICES[offset];
	vec3 direction = normalize(VERTEX);
	vec3 axisA = cross(direction, vec3(0.0,1.0,0.0));
	if (length(axisA) < 0.001) {
		axisA = cross(direction, vec3(0.0,0.0,1.0));
	}
	axisA = normalize(axisA);
	vec3 axisB = cross(direction, axisA);
	vec3 vertexOnSphere = (axisA * vertexOnPlane.y + axisB * vertexOnPlane.x) * 250.0;

	vec4 v = (PROJECTION_MATRIX * MODELVIEW_MATRIX * vec4(VERTEX + vertexOnSphere, 1.0));

	vec4 screenPos = computeScreenPos(v);
	vec2 screenSpaceUV = screenPos.xy / screenPos.w;
	vec4 backgroundCol = texture(MainTex, screenSpaceUV.xy);
	
	float backgroundBrightness = saturate(dot(backgroundCol.rgb, vec3(1.0)) / 3.0);
	float starBrightness = (1.0 - backgroundBrightness);
	vec4 starCol = texture(Spectrum, vec2(UV.y, 0.5));
	vec4 clusterCol = texture(ClusterSpectrum, vec2(UV.y, 0.5));
	col = vec4(mix(starCol.rgb, clusterCol.rgb, COLOR.r), starBrightness);
	brightnessFalloff = UV.x;
	vScreen = screenPos.xyz / v.w;

	vPos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	POSITION = v;
}

void fragment() {
	float falloff = dot(vertexOnPlane, vertexOnPlane);
	if (falloff > 1.0) {
		discard;
	}
	
	vec4 screenCol = texture(MainTex, vScreen.xy);
	
	float b = 0.25 - falloff;
	b = saturate(b + 0.5);
	b *= b;
	
	vec3 rayPos = CAMERA_POSITION_WORLD;
	vec3 rayDir = normalize(vPos - rayPos);
	
	vec2 hit = raySphere(PlanetCentre, OceanRadius, rayPos, rayDir);
	float dstThroughOcean = hit.y;

	float oceanIntersectDepth = saturate(dstThroughOcean);
	
	float visibility = saturate(length(exp(-daytimeFade * screenCol.rgb))) * (1.0 - oceanIntersectDepth);

	ALBEDO = screenCol.rgb * (1.0 - visibility) + col.rgb * visibility;
	ALPHA = col.a * b * visibility;
}
