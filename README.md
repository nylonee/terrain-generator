# Project 1 (COMP30019 SEM2 2016)
## A Unity Project

A procedurally generated terrain, made using diamond-square algorithm (wrapping around to get the fourth point for edge cases).

Water is also generated as a transparent plane at a reasonable height of the generated terrain. The sun rotates around the terrain, and a custom Phong shader is used for lighting on both the water and terrain.

Navigation is in a 'flight simulator' style, the mouse controls pitch and yaw, WASD controls movement, and QE controls the roll of the camera. You can't go through terrain or out of the boundary of the terrain, but you can fly up to the sky.
