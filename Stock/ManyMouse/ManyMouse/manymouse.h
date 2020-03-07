/*
 * ManyMouse main header. Include this from your app.
 *
 * Please see the file LICENSE.txt in the source's root directory.
 *
 *  This file written by Ryan C. Gordon.
 */

#ifndef _INCLUDE_MANYMOUSE_H_
#define _INCLUDE_MANYMOUSE_H_

#ifdef __cplusplus
extern "C" {
#endif

#ifndef DECLSPEC
#if defined(_WIN32)
#define DECLSPEC __declspec(dllexport)
#else
#define DECLSPEC
#endif
#endif

#define MANYMOUSE_VERSION "0.0.3"

typedef enum
{
    MANYMOUSE_EVENT_ABSMOTION = 0,
    MANYMOUSE_EVENT_RELMOTION,
    MANYMOUSE_EVENT_BUTTON,
    MANYMOUSE_EVENT_SCROLL,
    MANYMOUSE_EVENT_DISCONNECT,
    MANYMOUSE_EVENT_MAX
} ManyMouseEventType;

typedef struct
{
    ManyMouseEventType type;
    unsigned int device;
    unsigned int item;
    int value;
    int minval;
    int maxval;
} ManyMouseEvent;


/* internal use only. */
typedef struct
{
    const char *driver_name;
    int (*init)(void);
    void (*quit)(void);
    const char *(*name)(unsigned int index);
    int (*poll)(ManyMouseEvent *event);
} ManyMouseDriver;


DECLSPEC int ManyMouse_Init(void);
DECLSPEC const char *ManyMouse_DriverName(void);
DECLSPEC void ManyMouse_Quit(void);
DECLSPEC const char *ManyMouse_DeviceName(unsigned int index);
DECLSPEC int ManyMouse_PollEvent(ManyMouseEvent *event);

#undef DECLSPEC

#ifdef __cplusplus
}
#endif

#endif  /* !defined _INCLUDE_MANYMOUSE_H_ */

/* end of manymouse.h ... */

