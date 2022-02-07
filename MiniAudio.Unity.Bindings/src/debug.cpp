#include "../headers/debug.h"

log_info_ptr debug_log;
log_warn_ptr debug_warn;
log_error_ptr debug_error;

void InitializeLogger(log_info_ptr log_ptr, log_warn_ptr warn_ptr, log_error_ptr error_ptr) {
    debug_log = log_ptr;
    debug_warn = warn_ptr;
    debug_error = error_ptr;
}