from perlin_noise import PerlinNoise
import sys

args = sys.argv[1].split()
octaves_input = int(args[0].split(",")[0]) + int(args[0].split(",")[1]) / (len(args[0].split(",")[1]) * 10)
seed_input = int(args[1])
fromX = int(args[2])
fromY = int(args[3])
toX = int(args[4])
toY = int(args[5])

noise = PerlinNoise(octaves = octaves_input, seed = seed_input)

for i in range(fromX, toX + 1):
    print(str([round(noise([i, j]), 2) for j in range(fromY, toY)]))