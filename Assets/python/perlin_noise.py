from perlin_noise import PerlinNoise
import sys

args = sys.argv[1].split()
octaves_input = int(args[0].split(",")[0]) + int(args[0].split(",")[1]) / (len(args[0].split(",")[1]) * 10)
seed_input = int(args[1])

noise = PerlinNoise(octaves = octaves_input, seed = seed_input)

for i in range(10):
    print(str([round(noise([i, j]), 2) for j in range(10)]))