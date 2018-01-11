//
//     VapingDB
//     Copyright (C) 2013 - 2017, Int6ware - http://www.int6ware.com
//     This file is part of VapingDB project. Unauthorized copying of this file, via any medium is strictly prohibited.
//     VapingDB or its components/sources can not be copied and/or distributed without the express permission of Int6ware.
//
'use strict';

module.exports = function (grunt) {
  require('time-grunt')(grunt);

  grunt.initConfig({
    shell: {
      install_vcpkg: 'git clone https://github.com/Microsoft/vcpkg.git build/vcpkg'
    }
  });

  // load all plugins.
  require('load-grunt-tasks')(grunt);

  // task steps.
  grunt.registerTask('deps', ['shell:install_vcpkg']);

  // build tasks.
  grunt.registerTask('build', ['deps']);

  // default task.
  grunt.registerTask('default', ['build']);
};
