#include "../headers/debug.h"

void InitializeLogger(debug_function_ptr function_ptr) {
    debug_log = function_ptr;
}

int add(int a, int b) {
    debug_log("Hey I added something from C++ and logged it into Unity.");
    return a + b;
}