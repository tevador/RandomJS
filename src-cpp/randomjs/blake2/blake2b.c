#include "blake2-config.h"

#if defined(HAVE_NEON)
#include "blake2b-neon.c"
#elif defined(HAVE_SSE2)
#include "blake2b-sse.c"
#else
#if defined(__GNUC__)
#warning "Using reference Blake2b code"
#else
#define STRINGIFY(x) #x
#define TOSTRING(x) STRINGIFY(x)
#pragma message (__FILE__ "(" TOSTRING(__LINE__) ") : warning: Using reference Blake2b code")
#undef TOSTRING
#endif
#include "blake2b-ref.c"
#endif
