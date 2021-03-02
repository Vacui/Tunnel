public static class DirectionUtils {

    public static Direction Opposite(this Direction dir) {
        switch (dir) {
            default:
            case Direction.Up: return Direction.Down;
            case Direction.Right: return Direction.Left;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
        }
    }

    public static void DirectionToCoord(Direction moveDirection, ref int x, ref int z) {
        if (moveDirection != Direction.NULL) {
            switch (moveDirection) {
                default:
                case Direction.Up: z++; break;
                case Direction.Right: x++; break;
                case Direction.Down: z--; break;
                case Direction.Left: x--; break;
            }
        } else {
            x = -1;
            z = -1;
        }
    }
}