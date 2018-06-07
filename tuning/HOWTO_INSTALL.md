# Quik howto irace instructions (Linux only)
More details can be found in the latest user guide at: https://cran.r-project.org/web/packages/irace/vignettes/irace-package.pdf



### Install R
Install R on your system. Ubuntu: `sudo apt-get install r-base`.


### Install irace
* Open R.
* Type "install.packages("irace")"
* Still within R, get the installation path of the irace package on your system, type "system.file(package="irace")". Copy that location. Close R with "quit()", you do not need to save the workspace.


### Update your path so irace can be used directly from your shell
* Add this to your .bashrc or .bashprofile:
  -------------------------
  # Replace <IRACE_HOME> with the irace installation path you copied earlier.
  export IRACE_HOME="<installation_path>"
  export PATH=${IRACE_HOME}/bin/:$PATH

  # Tell R where to find R_LIBS_USER
  # Use the following line only if local installation was forced
  export R_LIBS=${R_LIBS_USER}:${R_LIBS}
  ------------------------
* Open a new terminal, and verify that everything is found by typing "irace --help".


### Run irace
* cd tuning
* Make "target-runner" executable if it is not with "chmod +x target-runner"
* Run irace with "irace".
For a quick trial run, reduce the budget (set to 5000 in scenario file) with "--max-experiments 500"
