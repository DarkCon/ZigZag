public static class Direction {
    public const int RIGHT = 0;
    public const int FORWARD = 1;
    
    public static int Next(int dir) {
        if (dir == RIGHT)
            return FORWARD;
        return RIGHT;
    }
}