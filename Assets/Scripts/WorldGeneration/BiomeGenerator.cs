using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public bool useDomainWarping = true;

    public DomainWarping domainWarping;
    public NoiseSettings biomeNoiseSettings;
    public BlockLayerHandler startLayerHandler;
    public List<BlockLayerHandler> additionalLayerHandlers;

    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
    {
        biomeNoiseSettings.worldOffset = mapSeedOffset;
        int groundPosition = GetSurfaceHeightNoice(data.worldPosition.x + x, data.worldPosition.z + z, data.chunkHeight);
        for (int y = 0; y < data.chunkHeight; y++)
        {
            startLayerHandler.Handle(data, x, y, z, groundPosition, mapSeedOffset);
        }
        foreach (var layerHandler in additionalLayerHandlers)
        {
            layerHandler.Handle(data, x, data.worldPosition.y, z, groundPosition, mapSeedOffset);
        }
        return data;
    }

    private int GetSurfaceHeightNoice(int x, int z, int chunkHeight)
    {
        float terrainHeight;

        if(useDomainWarping)
        {
            terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);
        } else
        {
            terrainHeight = GameNoice.OctavePerlin(x, z, biomeNoiseSettings);
        }

        terrainHeight = GameNoice.Redistribution(terrainHeight, biomeNoiseSettings);
        int surfaceHeight = GameNoice.RemapValue01ToInt(terrainHeight, 0, chunkHeight);
        return surfaceHeight;
    }
}
