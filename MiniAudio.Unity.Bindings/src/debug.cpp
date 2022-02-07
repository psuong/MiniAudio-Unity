#include "../headers/debug.h"

void InitializeLogger(log_info_ptr log_ptr, log_warn_ptr warn_ptr, log_error_ptr error_ptr) {
    debug_log = log_ptr;
    debug_warn = warn_ptr;
    debug_error = error_ptr;
}