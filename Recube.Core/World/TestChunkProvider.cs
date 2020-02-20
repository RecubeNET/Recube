using System;
using Recube.Api.World;
using Recube.Api.World.Generator;

namespace Recube.Core.World
{
    public class TestChunkProvider : IChunkProvider
    {
        private const float Noisescale = 0.5F;
        private readonly FastNoise _terrainNoise = new FastNoise(622966836);
        private readonly FastNoise _caveNoise = new FastNoise(622966836);
        public TestChunkProvider()
        {
            _terrainNoise.SetFrequency(0.009F);
            _terrainNoise.SetInterp(FastNoise.Interp.Quintic);
            _terrainNoise.SetFractalType(FastNoise.FractalType.FBM);
            _terrainNoise.SetFractalOctaves(5);
            _terrainNoise.SetFractalLacunarity(2);
            _terrainNoise.SetFractalGain(0.5F);
            _terrainNoise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
        }

        public IChunk GetChunk(int chunkX, int chunkZ)
        {
            Chunk chunk = new Chunk(chunkX, chunkZ);
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    float Y = _terrainNoise.GetNoise((x + chunkX * 16) * Noisescale, (z + chunkZ * 16) * Noisescale);
                    Y = 128 + Y * 128;
                    for (int y = 0; y < 256; y++)
                    {
                        float caveY = _caveNoise.GetNoise(x + chunkX * 16, y + 256 % 16 * 16, z + chunkZ * 16);
                        
                        if (y < Y)
                        {
                            chunk.SetType(x, y, z, 1);
                        }

                        if (y == (int) MathF.Floor(Y))
                        {
                            chunk.SetType(x, y, z, 9);
                        }

                        if (caveY > 0.8)
                        {
                            chunk.SetType(x,y,z, 0);
                        }
                    }
                }
            }

            return chunk;
        }
    }
}