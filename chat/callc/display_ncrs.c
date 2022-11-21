#include "jni.h"
#include "ncurses.h"
#include "chat_NcursesDisplay.h"

WINDOW *canvases[16];
unsigned short r = 0,c = 0;

JNIEXPORT void JNICALL Java_chat_NcursesDisplay_initscr
  (JNIEnv *env, jobject obj){
    canvases[0] = initscr();
  }
JNIEXPORT jint JNICALL Java_chat_NcursesDisplay_endwin
  (JNIEnv *env, jobject obj){
    return endwin();
  }
JNIEXPORT jint JNICALL Java_chat_NcursesDisplay_mvprintw
  (JNIEnv *env, jobject obj, jint y, jint x, jstring t){
    return mvwprintw(stdscr, y, x, "%s", (const char *)((*env)->GetStringUTFChars(env, t, FALSE)));
  }
JNIEXPORT jstring JNICALL Java_chat_NcursesDisplay_getscr
  (JNIEnv *env, jobject obj){
    char *text;
    getstr(text);
    return (*env)->NewStringUTF(env, text);
  }
JNIEXPORT jshort JNICALL Java_chat_NcursesDisplay_getmaxy
  (JNIEnv *env, jobject obj, jint id){
    return getmaxy(canvases[id]);
  }
JNIEXPORT jshort JNICALL Java_chat_NcursesDisplay_getmaxx
  (JNIEnv *env, jobject obj, jint id){
    return getmaxx(canvases[id]);
  }
JNIEXPORT void JNICALL Java_chat_NcursesDisplay_exit_1curses
  (JNIEnv *env, jobject obj, jint m){
    exit_curses(m);
  }
JNIEXPORT jint JNICALL Java_chat_NcursesDisplay_getch
  (JNIEnv *env, jobject obj){
    return getch();
  }
JNIEXPORT jint JNICALL Java_chat_NcursesDisplay_wrefresh
  (JNIEnv *env, jobject obj, jint id){
    return wrefresh(canvases[id]);
  }
JNIEXPORT jint JNICALL Java_chat_NcursesDisplay_wclear
  (JNIEnv *env, jobject obj, jint id){
    return wclear(canvases[id]);
  }

