#!/usr/bin/env bash

DESTDIR="${DESTDIR:-/usr/local/}"
generic_src="$(readlink -f $(dirname "${BASH_SOURCE[0]}"))"

. ${generic_src}/../lib.sh

if [ ! -d "${OTD_BUILD_DIR}" ]; then
  echo "OTD_BUILD_DIR is not set, please build OpenTabletDriver and set this variable."
  exit 1
fi

# Scripts
mkdir -p "${DESTDIR}/bin"
install -m 0755 "${generic_src}/Scripts/otd" "${DESTDIR}/bin/otd"
install -m 0755 "${generic_src}/Scripts/otd-daemon" "${DESTDIR}/bin/otd-daemon"
install -m 0755 "${generic_src}/Scripts/otd-gui" "${DESTDIR}/bin/otd-gui"

# modprobe
mkdir -p "${DESTDIR}/lib/modprobe.d"
install -m 0644 "${generic_src}/modprobe/99-opentabletdriver.conf" "${DESTDIR}/lib/modprobe.d/99-opentabletdriver.conf"

# systemd user
mkdir -p "${DESTDIR}/lib/systemd/user"
install -m 0644 "${generic_src}/systemd-user/opentabletdriver.service" "${DESTDIR}/lib/systemd/user/opentabletdriver.service"

# manpages
mkdir -p "${DESTDIR}/share/man/man8"
gzip -c "${generic_src}/manpages/opentabletdriver.8" > "${DESTDIR}/share/man/man8/opentabletdriver.8.gz"

# license
mkdir -p "${DESTDIR}/share/doc/opentabletdriver"
install -m 0644 "${REPO_ROOT}/LICENSE" "${DESTDIR}/share/doc/opentabletdriver/LICENSE"

# pixmaps
copy_pixmap_assets "${DESTDIR}/share/pixmaps"

# udev rules
mkdir -p "${DESTDIR}/lib/udev/rules.d"
mv "${OTD_BUILD_DIR}/99-opentabletdriver.rules" "${DESTDIR}/lib/udev/rules.d/99-opentabletdriver.rules"

# desktop entry
generate_desktop_file "${DESTDIR}/share/applications/opentabletdriver.desktop"

# OpenTabletDriver
mkdir -p "${DESTDIR}/lib/opentabletdriver"
cp -r "${OTD_BUILD_DIR}"/* "${DESTDIR}/lib/opentabletdriver"
