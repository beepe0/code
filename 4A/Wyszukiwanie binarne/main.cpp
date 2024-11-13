#include "iostream"

#define length(array) (sizeof(array) / sizeof(*array))

size_t find_number(int target, int numbers[], size_t length) {
    int l = 0;
    int p = length - 1;
    int sr = (l + p) / 2;

    while (l <= p) {
        if (numbers[sr] == target) return sr;
        else if (numbers[sr] > target) p = sr - 1;
        else l = sr + 1;

        sr = (l + p) / 2;
    }

    return -1;
}

int main() {
    int numbers[] = {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47};
    for (int i = 0; i < length(numbers); i++) {
        std::cout << numbers[i] << " = " << find_number(numbers[i], numbers, length(numbers)) << std::endl;
    }
}
