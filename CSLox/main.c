#include "stdio.h"
#include "stdbool.h"

int main (void) {
    bool y = true;
    int x = !y ? 2 : 3;
    printf("%d", x);
    return 0;
}
