const gulp = require('gulp');
const postcss = require('gulp-postcss');
const sourcemaps = require('gulp-sourcemaps');
const cleanCSS = require('gulp-clean-css');
const purgecss = require('gulp-purgecss');

gulp.task('css:dev', () => {
	return gulp.src('./tailwind.css')
		.pipe(sourcemaps.init())
		.pipe(postcss([
			require('precss'),
			require('tailwindcss'),
			require('autoprefixer')
		]))
		.pipe(gulp.dest('./wwwroot/'));
});

gulp.task('css:prod', () => {
	return gulp.src('./tailwind.css')
		.pipe(sourcemaps.init())
		.pipe(postcss([
			require('precss'),
			require('tailwindcss'),
			require('autoprefixer')
		]))
		.pipe(purgecss({ content: ['**/*.html', '**/*.razor'] }))
		.pipe(cleanCSS({ level: 2 }))
		.pipe(sourcemaps.write('.'))
		.pipe(gulp.dest('./wwwroot/'));
});
