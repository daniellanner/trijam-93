
#define CLIP_DISTANCE 10

void ClipPlane(float3 worldposition, float3 facing, float distance)
{
	//calculate signed distance to plane
	float d = dot(worldposition, facing);
	d = d + distance;
	//discard surface above plane
	clip(-d);
}

void ClipCircle(float3 worldposition, float3 facing, float distance)
{
	float dist =
		worldposition.x * worldposition.x +
		worldposition.y * worldposition.y +
		worldposition.z * worldposition.z;


	//discard surface above plane
	clip(distance * distance - dist);
}