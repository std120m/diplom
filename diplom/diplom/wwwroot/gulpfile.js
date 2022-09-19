var gulp = require('gulp'),
    pagebuilder = require('gulp-pagebuilder');

gulp.task('default', function () {
    return gulp.src('src/*.html')
        .pipe(pagebuilder('src'))
        .pipe(gulp.dest('build/'));
});