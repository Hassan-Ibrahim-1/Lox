#include <stdio.h>

int fib(int n) {
    if (n <= 1) return n;
    return fib(n - 2) + fib(n - 1);
}

int main(void) {
    for (int i = 0; i < 20; i++) {
        printf("%d\n", fib(i));
    }
    return 0;
}
