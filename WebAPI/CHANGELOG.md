## [1.10.2](https://github.com/AESInsight/Backend/compare/v1.10.1...v1.10.2) (2025-05-26)


### Bug Fixes

* fix backend deployment ([0f9e2f4](https://github.com/AESInsight/Backend/commit/0f9e2f444c4fa18ae3b8d4c6b5b5d2fd7ff37794))

## [1.10.1](https://github.com/AESInsight/Backend/compare/v1.10.0...v1.10.1) (2025-05-13)


### Bug Fixes

* fix reset password ([b61954a](https://github.com/AESInsight/Backend/commit/b61954a1a6c9a1631edefd161fb1dc734082b5ac))

# [1.10.0](https://github.com/AESInsight/Backend/compare/v1.9.4...v1.10.0) (2025-05-09)


### Features

* adds employeeDTO and EmployeeModel ([40d7eac](https://github.com/AESInsight/Backend/commit/40d7eacbb2f5fc23f6b60606d540fa01f1667a07))

## [1.9.4](https://github.com/AESInsight/Backend/compare/v1.9.3...v1.9.4) (2025-05-08)


### Bug Fixes

* query getAverageSalaryForJobsIn{industry} and getAllIndustries by removing EmployeeModelEmployeeID ([855626f](https://github.com/AESInsight/Backend/commit/855626fdb2cc0c95971fd77ab9746de30fdb0778))

## [1.9.3](https://github.com/AESInsight/Backend/compare/v1.9.2...v1.9.3) (2025-05-07)


### Bug Fixes

* deploy using correct paths ([e189cae](https://github.com/AESInsight/Backend/commit/e189cae8d8f2358989576ee10088390c507a947e))

## [1.9.2](https://github.com/AESInsight/Backend/compare/v1.9.1...v1.9.2) (2025-05-07)


### Bug Fixes

* fixes azure deployment in relation to environments ([ae2b553](https://github.com/AESInsight/Backend/commit/ae2b553cd4462c60acc6f95c0a9feed7e9eddf16))

## [1.9.1](https://github.com/AESInsight/Backend/compare/v1.9.0...v1.9.1) (2025-04-30)


### Bug Fixes

* database update ([567dd02](https://github.com/AESInsight/Backend/commit/567dd0237d01ed138777930371309322e805e14f))

# [1.9.0](https://github.com/AESInsight/Backend/compare/v1.8.1...v1.9.0) (2025-04-29)


### Bug Fixes

* rename CI path ([5575058](https://github.com/AESInsight/Backend/commit/55750588079d08afd955a926c6bd56ee4101dc23))
* Renames project reference in CI ([967b3a8](https://github.com/AESInsight/Backend/commit/967b3a84a9ca738a34015d3be52bae94d019ab30))
* renames sln file ([a06889b](https://github.com/AESInsight/Backend/commit/a06889b500b2cad9698e8648c0d5b9119d3add86))


### Features

* death commit ([3451553](https://github.com/AESInsight/Backend/commit/34515534cf56f75583219b96691f7205f573c784))

## [1.8.1](https://github.com/AESInsight/Backend/compare/v1.8.0...v1.8.1) (2025-04-22)


### Bug Fixes

* Fixes infinitely increasing ID's, so they'll continue from highest existing number instead. ([3213374](https://github.com/AESInsight/Backend/commit/32133740775d365d0e20447e1575a048fa5c8576))
* Fixes the auth/login controller ([e229787](https://github.com/AESInsight/Backend/commit/e229787303c86dd7a8335d4364981bab5509a2d8))

# [1.8.0](https://github.com/AESInsight/Backend/compare/v1.7.0...v1.8.0) (2025-04-22)


### Bug Fixes

* change EmployeeModel ([4546e0e](https://github.com/AESInsight/Backend/commit/4546e0ea19b6bc8c237eb2a1e97b2fa5cdd0c644))


### Features

* Adds all salary ([b6fd5df](https://github.com/AESInsight/Backend/commit/b6fd5dfba29e5cccb564005adf5470828c5c671e))

# [1.7.0](https://github.com/AESInsight/Backend/compare/v1.6.0...v1.7.0) (2025-04-22)


### Features

* adds endpoints to fetch data for chart 2 - its now possible to fetch all industries, the and the average salary for eaach jobtitlte in that industry based on gender ([141a0d7](https://github.com/AESInsight/Backend/commit/141a0d7a072589848b6644f0293e8524168acb08))
* setup of querys for chart 1, we can now fetch average salary for gender overtime and average salary for gender by jobtitle ([cb21069](https://github.com/AESInsight/Backend/commit/cb210696b25136c93389c602307468ecaa6eae27))

# [1.6.0](https://github.com/AESInsight/Backend/compare/v1.5.2...v1.6.0) (2025-04-21)


### Bug Fixes

* issue with timestamp ([e9f10c3](https://github.com/AESInsight/Backend/commit/e9f10c31f46d22d8fd22c02e7b489848f7842c4c))
* removes salary from emp dto ([d162195](https://github.com/AESInsight/Backend/commit/d162195589f54296562e182b8e718361cd18afaa))


### Features

* adds migration for salary ([6fafc59](https://github.com/AESInsight/Backend/commit/6fafc59e80f205163a09c2d4d50cfea6deb03f31))

## [1.5.2](https://github.com/AESInsight/Backend/compare/v1.5.1...v1.5.2) (2025-04-11)


### Bug Fixes

* Improves controllers and general codespace. Adds DTOs. ([77432fa](https://github.com/AESInsight/Backend/commit/77432faa6aaacbd583bf674d8743180303e2cbfa))
* Removes all authorization from employee controller ([caef41e](https://github.com/AESInsight/Backend/commit/caef41ec356f8c39723d131e3f2f39fb0f5d1203))

## [1.5.1](https://github.com/AESInsight/Backend/compare/v1.5.0...v1.5.1) (2025-04-11)


### Bug Fixes

* app name ([56f4f45](https://github.com/AESInsight/Backend/commit/56f4f45861dfd0f28e791f774e6860f49f6aaff6))

# [1.5.0](https://github.com/AESInsight/Backend/compare/v1.4.3...v1.5.0) (2025-04-11)


### Bug Fixes

* Fixes email sending out mail ([b578738](https://github.com/AESInsight/Backend/commit/b578738c022fb575baf96d150a38d5cd92b7f548))
* Fixes the reset password, so we can test it ([534ed18](https://github.com/AESInsight/Backend/commit/534ed18fb2d983eb9d68c877f8e186407d160386))
* fixes this branch, so it matches develope to avoid conflicts ([40ecff7](https://github.com/AESInsight/Backend/commit/40ecff7dbdc0ef1b431bc4a0139ac36419b40f39))
* Update Services/EmailService.cs ([96e155f](https://github.com/AESInsight/Backend/commit/96e155f07588aa012102fc66b91b35c5663373df))


### Features

* Adds confirmpassword and removes email from Passwordreset/reset ([1230389](https://github.com/AESInsight/Backend/commit/123038936618fec9c68a8dba93ffbbdcf23b3a96))
* Adds correct mail instructions ([288ae4c](https://github.com/AESInsight/Backend/commit/288ae4c5e3885f05830196c0789e5b2aaf05ec1d))
* Adds reset password function ([fb16cbf](https://github.com/AESInsight/Backend/commit/fb16cbf0a9d6e057304807e23e1e30ba3e167638))

## [1.4.3](https://github.com/AESInsight/Backend/compare/v1.4.2...v1.4.3) (2025-04-08)


### Bug Fixes

* fix deploy file ([92496da](https://github.com/AESInsight/Backend/commit/92496da7b88457cb1f7866965dd8c037c64b66bf))

## [1.4.2](https://github.com/AESInsight/Backend/compare/v1.4.1...v1.4.2) (2025-04-08)


### Bug Fixes

* Removes build errors ([78c21dc](https://github.com/AESInsight/Backend/commit/78c21dcbf78014f4a49b3894405797724aff8457))

## [1.4.1](https://github.com/AESInsight/Backend/compare/v1.4.0...v1.4.1) (2025-04-08)


### Bug Fixes

* updates deploy file path ([a31b427](https://github.com/AESInsight/Backend/commit/a31b427122ef0e227af929633ca6270c225fac1d))

# [1.4.0](https://github.com/AESInsight/Backend/compare/v1.3.1...v1.4.0) (2025-04-08)


### Bug Fixes

* Adds correct error handling for CVR ([11a90c7](https://github.com/AESInsight/Backend/commit/11a90c7e0a6f3e8d31acc04e4da80f293119d57c))


### Features

* Consolidates company post requests ([a4de28e](https://github.com/AESInsight/Backend/commit/a4de28eac1b93c99b1ece7014743b17ad63ff7aa))
* Further consolidates and refactors company controller and service. ([cf4792d](https://github.com/AESInsight/Backend/commit/cf4792dab9924b413f707ec04d756adae2e3604e))
* Merge pull request [#48](https://github.com/AESInsight/Backend/issues/48) from AESInsight/feat/37/improve-company-controller ([af59bb0](https://github.com/AESInsight/Backend/commit/af59bb0ec92c12915cb3c026b3b8568f46aa2676))

## [1.3.1](https://github.com/AESInsight/Backend/compare/v1.3.0...v1.3.1) (2025-04-08)


### Bug Fixes

* cleans code in middleware ([9bda382](https://github.com/AESInsight/Backend/commit/9bda382343dc18e93ca963a338043febf09012bd))

# [1.3.0](https://github.com/AESInsight/Backend/compare/v1.2.0...v1.3.0) (2025-04-01)


### Features

* Merge pull request [#36](https://github.com/AESInsight/Backend/issues/36) from AESInsight/feat/35/authorize-put-delete-endpoints ([6e3f314](https://github.com/AESInsight/Backend/commit/6e3f314fe2069475a4e1e25fb8b7c19139451bed))
* Updates put and delete requests to include ASP.NET authorization attribute for "Admin" role. ([c107ee6](https://github.com/AESInsight/Backend/commit/c107ee64d6eb79bc5676d1cb2ecc29caa18961d2))

# [1.2.0](https://github.com/AESInsight/Backend/compare/v1.1.0...v1.2.0) (2025-04-01)


### Bug Fixes

* removes warnings ([af969f7](https://github.com/AESInsight/Backend/commit/af969f728c74e0e08df58601a781a119b7dfc7c9))


### Features

* implements employee/update endpoint ([12ea687](https://github.com/AESInsight/Backend/commit/12ea687ae96f649cc0e2d38d4209dee6645eacf5))
* Implements endpoint to delete employee by ID. ([fdf7e26](https://github.com/AESInsight/Backend/commit/fdf7e26f565f4a9f28261c35c2e618b62aefb6b4))
* Implements endpoint to get employees by company ID ([143f0c2](https://github.com/AESInsight/Backend/commit/143f0c20b932df00cae3d74aa7b87e9c10e5bf43))
* Merge pull request [#33](https://github.com/AESInsight/Backend/issues/33) from AESInsight/feat/25/improve-employee-controller ([096dd24](https://github.com/AESInsight/Backend/commit/096dd24bbc423cb0378e9114b25fa9315bf4b51b))

# [1.1.0](https://github.com/AESInsight/Backend/compare/v1.0.0...v1.1.0) (2025-04-01)


### Features

* Adds JFToken ([a84f0f7](https://github.com/AESInsight/Backend/commit/a84f0f79f3f71415c38ae1144b66d8c43b5079c0))

# 1.0.0 (2025-03-14)


### Bug Fixes

* actual fix ([e501deb](https://github.com/AESInsight/Backend/commit/e501deb13f6aeadd342c1d71789c0a3c90362d24))
* fixes release workflow ([f1d0eaa](https://github.com/AESInsight/Backend/commit/f1d0eaa86fe4204a5d187d737706c4d124905484))


### Features

* adds more checks to pipeline ([b2d1365](https://github.com/AESInsight/Backend/commit/b2d1365acf0651226baac593456a8ffe04937003))
* Creates backend project and gitignore ([dfb5b07](https://github.com/AESInsight/Backend/commit/dfb5b07e0413133c02a4d193431448fa9cb11732))
