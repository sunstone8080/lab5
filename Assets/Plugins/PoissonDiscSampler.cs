using System.Collections.Generic;
using UnityEngine;

namespace AvoiderPlugin
{
    public class PoissonDiscSampler
    {
        const int K = 30;

        readonly float width, height, radius2, cellSize;
        readonly Vector2[,] grid;
        readonly List<Vector2> active = new List<Vector2>();

        public PoissonDiscSampler(float width, float height, float radius)
        {
            this.width = width;
            this.height = height;
            radius2 = radius * radius;
            cellSize = radius / Mathf.Sqrt(2f);
            grid = new Vector2[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize)];


        }

        public IEnumerable<Vector2> Samples()
        {
            yield return AddSample(new Vector2(Random.Range(0f, width * 0.99f), Random.Range(0f, height * 0.99f)));

            while (active.Count > 0)

            {


                int i = Random.Range(0, active.Count);
                Vector2 sample = active[i];
                bool found = false;

                for (int j = 0; j < K; j++)
                {
                    float angle = Random.value * 2f * Mathf.PI;
                    float dist = Mathf.Sqrt(Random.value * 3f * radius2 + radius2);
                    Vector2 candidate = sample + dist * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));


                    if (candidate.x >= 0 && candidate.y >= 0 && candidate.x < width && candidate.y < height && FarEnough(candidate))
                    {
                        found = true;
                        yield return AddSample(candidate);
                        break;


                    }

                }

                if (!found)
                {
                    active[i] = active[active.Count - 1];
                    active.RemoveAt(active.Count - 1);


                }
            }
        }

        bool FarEnough(Vector2 p)
        {
            int cx = (int)(p.x / cellSize);
            int cy = (int)(p.y / cellSize);

            for (int y = Mathf.Max(cy - 2, 0); y <= Mathf.Min(cy + 2, grid.GetLength(1) - 1); y++)
            {
                for (int x = Mathf.Max(cx - 2, 0); x <= Mathf.Min(cx + 2, grid.GetLength(0) - 1); x++)
                {
                    Vector2 other = grid[x, y];

                    if (other != Vector2.zero && (other - p).sqrMagnitude < radius2)
                    {
                        return false;


                    }

                }

            }

            return true;
        }

        Vector2 AddSample(Vector2 p)
        {
            active.Add(p);
            grid[(int)(p.x / cellSize), (int)(p.y / cellSize)] = p;
            return p;


        }
    }
}