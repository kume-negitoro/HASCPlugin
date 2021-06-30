namespace HASCPlugin {
    static class Utils {
        // processingのmapと同じ
        public static double LinearMap(double value, double start1, double end1, double start2, double end2)
        {
            return start2 + (end2 - start2) * ((value - start1) / (end1 - start1));
        }
    }
}