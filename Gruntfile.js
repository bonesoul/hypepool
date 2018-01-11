//
//     VapingDB
//     Copyright (C) 2013 - 2017, Int6ware - http://www.int6ware.com
//     This file is part of VapingDB project. Unauthorized copying of this file, via any medium is strictly prohibited.
//     VapingDB or its components/sources can not be copied and/or distributed without the express permission of Int6ware.
//
'use strict';

module.exports = function (grunt) {
  //require('time-grunt')(grunt);

  grunt.initConfig({
    shell: {
      clone_vcpkg: {
        command: 'git clone https://github.com/Microsoft/vcpkg.git build/vcpkg',
        options: {
          failOnError: false
        }
      },
      install_vcpkg: 'build\\vcpkg\\bootstrap-vcpkg.bat',
      install_packages: 'build\\vcpkg\\vcpkg install boost-system libsodium',
      cook_libcryptonote_deps: 'build\\vcpkg\\vcpkg export --nuget --nuget-id=libcryptonote.x86.deps --nuget-version=0.0.1 boost-system',
      cook_libmultihash_deps: 'build\\vcpkg\\vcpkg export --nuget --nuget-id=libmultihash.x86.deps --nuget-version=0.0.1 libsodium',
    }
  });

  // load all plugins.
  require('load-grunt-tasks')(grunt);

  // task steps.
  grunt.registerTask('vcpkg', ['shell:clone_vcpkg', 'shell:install_vcpkg']);
  grunt.registerTask('cook_dependencies', ['shell:install_packages', 'shell:cook_libcryptonote_deps', 'shell:cook_libmultihash_deps']);

  // build tasks.
  grunt.registerTask('deps', ['vcpkg', 'cook_dependencies']);
  grunt.registerTask('build', []);

  // default task.
  grunt.registerTask('default', ['build']);
};
