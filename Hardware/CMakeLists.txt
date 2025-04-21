cmake_minimum_required(VERSION 3.12)

# Pull in SDK (must be before project)
include(pico_sdk_import.cmake)
include(pico_extras_import_optional.cmake)

project(imu_pico_output C CXX ASM)

set(CMAKE_C_STANDARD 11)
set(CMAKE_CXX_STANDARD 17)

# initialize the sdk
pico_sdk_init()

if (TARGET tinyusb_device)
    add_executable(imu_pico_output
        imu_pico_output.c
    )

    # pull in common dependencies
    target_link_libraries(imu_pico_output pico_stdlib)
    target_link_libraries(imu_pico_output pico_stdlib hardware_i2c)

    # enable usb output, disable uart output
    pico_enable_stdio_usb(imu_pico_output 1)
    pico_enable_stdio_uart(imu_pico_output 0)

    # create map/bin/hex/uf2 file etc.
    pico_add_extra_outputs(imu_pico_output)

elseif(PICO_ON_DEVICE)
	message("Could not make! Is TinyUSB submodule initialized in the SDK? Was libusb installed with picotool?")
endif()
