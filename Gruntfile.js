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
        command: 'git clone https://github.com/Microsoft/vcpkg.git <%= vcpkg_dir %> && git pull',
        options: {
          failOnError: false
        }
      },
      install_vcpkg: '<%= vcpkg_dir %>\\bootstrap-vcpkg.bat',
      install_packages: '<%= vcpkg %> install boost',
      cook_libboost: '<%= vcpkg %> export --nuget --nuget-id=libboost --nuget-version=1.66.0 boost'
    }
  });

  // load all plugins.
  require('load-grunt-tasks')(grunt);

  // task steps.
  grunt.registerTask('vcpkg', ['shell:clone_vcpkg', 'shell:install_vcpkg']);
  grunt.registerTask('cook_windows_deps', ['shell:install_packages', 'shell:cook_libboost']);

  // build tasks.
  grunt.registerTask('cook-deps', ['vcpkg', 'cook_windows_deps']);
  grunt.registerTask('build', []);

  // default task.
  grunt.registerTask('default', ['build']);
};
