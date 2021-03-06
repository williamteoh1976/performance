// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Adapted from spectral-norm C# .NET Core program
// http://benchmarksgame.alioth.debian.org/u64q/program.php?test=spectralnorm&lang=csharpcore&id=1
// aka (as of 2017-09-01) rev 1.2 of https://alioth.debian.org/scm/viewvc.php/benchmarksgame/bench/spectralnorm/spectralnorm.csharp?root=benchmarksgame&view=log
// Best-scoring single-threaded C# .NET Core version as of 2017-09-01

/* The Computer Language Benchmarks Game
   http://benchmarksgame.alioth.debian.org/
 
   contributed by Isaac Gouy 
*/

using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using MicroBenchmarks;

namespace BenchmarksGame
{
    [BenchmarkCategory(Categories.CoreCLR, Categories.BenchmarksGame)]
    public class SpectralNorm_1
    {
        [Benchmark(Description = nameof(SpectralNorm_1))]
        public double RunBench() => Bench(100);

        [MethodImpl(MethodImplOptions.NoInlining)]
        double Bench(int n)
        {
            // create unit vector
            double[] u = new double[n];
            for (int i = 0; i < n; i++) u[i] = 1;

            // 20 steps of the power method
            double[] v = new double[n];
            for (int i = 0; i < n; i++) v[i] = 0;

            for (int i = 0; i < 10; i++)
            {
                MultiplyAtAv(n, u, v);
                MultiplyAtAv(n, v, u);
            }

            // B=AtA         A multiplied by A transposed
            // v.Bv /(v.v)   eigenvalue of v 
            double vBv = 0, vv = 0;
            for (int i = 0; i < n; i++)
            {
                vBv += u[i] * v[i];
                vv += v[i] * v[i];
            }

            return Math.Sqrt(vBv / vv);
        }


        /* return element i,j of infinite matrix A */
        double A(int i, int j)
        {
            return 1.0 / ((i + j) * (i + j + 1) / 2 + i + 1);
        }

        /* multiply vector v by matrix A */
        void MultiplyAv(int n, double[] v, double[] Av)
        {
            for (int i = 0; i < n; i++)
            {
                Av[i] = 0;
                for (int j = 0; j < n; j++) Av[i] += A(i, j) * v[j];
            }
        }

        /* multiply vector v by matrix A transposed */
        void MultiplyAtv(int n, double[] v, double[] Atv)
        {
            for (int i = 0; i < n; i++)
            {
                Atv[i] = 0;
                for (int j = 0; j < n; j++) Atv[i] += A(j, i) * v[j];
            }
        }

        /* multiply vector v by matrix A and then by matrix A transposed */
        void MultiplyAtAv(int n, double[] v, double[] AtAv)
        {
            double[] u = new double[n];
            MultiplyAv(n, v, u);
            MultiplyAtv(n, u, AtAv);
        }
    }
}
