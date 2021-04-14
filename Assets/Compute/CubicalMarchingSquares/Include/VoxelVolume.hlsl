#ifndef VOXEL_VOLUME__559277903
#define VOXEL_VOLUME__559277903

uint3 voxelDimensions;
float voxelSpacing;
float3 voxelVolumeToWorldSpaceOffset;

bool IsOutOfVoxelBounds(uint3 voxelID)
{
    return any(step(voxelDimensions, voxelID));
}

float3 VoxelToVoxelVolumeSpace(uint3 voxelID, float3 voxelSpacePosition = 0.0f)
{
    return voxelSpacing * ((voxelSpacePosition + voxelID) - 0.5f * voxelDimensions);
}

float3 VoxelVolumeToVoxelSpace(uint3 voxelID, float3 voxelVolumeSpacePosition = 0.0f)
{
    return(voxelVolumeSpacePosition / voxelSpacing + 0.5f * voxelDimensions) - voxelID;
}

float3 VoxelVolumeToWorldSpace(float3 voxelVolumeSpacePosition)
{
    return voxelVolumeSpacePosition + voxelVolumeToWorldSpaceOffset;
}

float3 WorldToVoxelVolumeSpace(float3 worldSpacePosition)
{
    return worldSpacePosition - voxelVolumeToWorldSpaceOffset;
}

#endif