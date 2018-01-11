//
//     hypepool
//     Copyright (C) 2013 - 2017, Int6ware - http://www.int6ware.com
//     This file is part of hypepool project. Unauthorized copying of this file, via any medium is strictly prohibited.
//     hypepool or its components/sources can not be copied and/or distributed without the express permission of Int6ware.
//
'use strict';

module.exports = function (grunt) {
  //require('time-grunt')(grunt);

  grunt.initConfig({
    vcpkg_dir: 'build\\vcpkg', // vcpkg dir.
    deps_dir: 'deps\\windows', // deps dir.
    vcpkg: 'build\\vcpkg\\vcpkg', // vcpkg executable.
    shell: {
      clone_vcpkg: {
        command: 'git clone https://github.com/Microsoft/vcpkg.git <%= vcpkg_dir %>',
        options: {
          failOnError: false
        }
      },
      install_vcpkg: '<%= vcpkg_dir %>\\bootstrap-vcpkg.bat',
      install_packages: '<%= vcpkg %> install boost-system libsodium',
      cook_libcryptonote_deps: '<%= vcpkg %> export --nuget --nuget-id=libcryptonote.x86.deps --nuget-version=0.0.1 boost-system',
      cook_libmultihash_deps: '<%= vcpkg %> export --nuget --nuget-id=libmultihash.x86.deps --nuget-version=0.0.1 libsodium',
    },
    copy: {
     packages: {
       files: [{
         expand: true,
         cwd: '<%= vcpkg_dir %>',
         src: ['*.nupkg'],
         dest: '<%= deps_dir %>'
       }]
     }
    }
  });

  // load all plugins.
  require('load-grunt-tasks')(grunt);

  // task steps.
  grunt.registerTask('vcpkg', ['shell:clone_vcpkg', 'shell:install_vcpkg']);
  grunt.registerTask('cook_windows_deps', ['shell:install_packages', 'shell:cook_libcryptonote_deps', 'shell:cook_libmultihash_deps']);
  grunt.registerTask('copy_windows_deps', ['copy:packages']);

  // build tasks.
  grunt.registerTask('cook-windows-deps', ['vcpkg', 'cook_windows_deps', 'copy_windows_deps']);
  grunt.registerTask('build', []);

  // default task.
  grunt.registerTask('default', ['build']);
};
