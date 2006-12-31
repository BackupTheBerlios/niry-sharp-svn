AC_INIT(README)
AC_CANONICAL_SYSTEM
AM_INIT_AUTOMAKE(niry-sharp, 1.0.0)
AM_MAINTAINER_MODE

AC_PROG_INSTALL

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
	AC_MSG_ERROR([You need to install pkg-config])
fi

AC_PATH_PROG(CSC, csc, no)
AC_PATH_PROG(MCS, gmcs, no)
AC_PATH_PROG(MONO, mono, no)

required_assemblies="Mono.Data.SqliteClient System.Data System.Drawing"
for asm in $required_assemblies; do
	AC_MSG_CHECKING([for $asm.dll])
	if test ! -e `$PKG_CONFIG --variable=prefix mono`/lib/mono/2.0/$asm.dll; then
		AC_MSG_RESULT([not found])
		AC_MSG_ERROR([missing required Mono Assembly: $asm.dll])
	else
		AC_MSG_RESULT([found])
	fi
done

CS="C#"
if test "x$CSC" = "xno" -a "x$MCS" = "xno"  ; then
	AC_MSG_ERROR([No $CS compiler found])
fi

if test "x$MCS" = "xno" ; then
	MCS=$CSC
fi

if test "x$MONO" = "xno"; then
	AC_MSG_ERROR([No mono runtime found])
fi

dnl gtk-sharp
GTKSHARP_REQUIRED_VERSION=2.10
PKG_CHECK_MODULES(GTKSHARP, gtk-sharp-2.0 >= $GTKSHARP_REQUIRED_VERSION)

AC_SUBST(MCS)

AC_OUTPUT([
Makefile
niry-sharp.pc
src/Makefile
src/AssemblyInfo.cs
src/Info.cs
])
