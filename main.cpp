/*
https://stackoverflow.com/questions/1220046/how-to-get-the-md5-hash-of-a-file-in-c
uses c++17 (because of filestream)
also uses boost and openssl
can be installed with "yay -S openssl" and "yay -S boost" on arch.

compilation needs -std=c++17 -lcryptob -lboost_iostreams

usage: md5 /path_to_folder/
*/
#include <openssl/md5.h>
#include <boost/iostreams/device/mapped_file.hpp>
#include <iomanip>
#include <sstream>
#include <string>
#include <iostream>
#include <filesystem>
namespace fs = std::filesystem; 

#include <chrono>
using std::chrono::high_resolution_clock;
using std::chrono::duration_cast;

const std::string md5_from_file(const std::string& path)
{
    unsigned char result[MD5_DIGEST_LENGTH];
    boost::iostreams::mapped_file_source src(path);
    MD5((unsigned char*)src.data(), src.size(), result);

    std::ostringstream sout;
    sout<<std::hex<<std::setfill('0');
    for(auto c: result) sout<<std::setw(2)<<(int)c;

    return sout.str();
}


int main(int argc, char *argv[])
{
    if (argc == 0) {

        std::cout << "Please provide a filepath as an argument.";
    }
    else {
        auto t1 = high_resolution_clock::now();
        int int_deleted_files = 0;
        std::string path(argv[1]);

        // for every file in the folder, do
        for (auto& p : fs::recursive_directory_iterator(path))
        {
            // checking if found path is a directory, if yes, then ignore
            std::string path_string = p.path().u8string();
            if (!fs::is_directory(path_string)) {

                // file info
                std::string path_string_ext = p.path().extension().u8string();
                std::string md5_hash = md5_from_file(&path_string[0]);

                // adding a "//" or "\"" to the end of the folderpath, depending on what os
                std::string directory;
                #if _WIN32 || _WIN64
                    size_t last_slash_idx = path_string.rfind('\\');
                #endif
                #if __linux__
                    size_t last_slash_idx = path_string.rfind('/');
                #endif

                if (std::string::npos != last_slash_idx)
                {
                    directory = path_string.substr(0, last_slash_idx);
                }

                // creating new full path
                #if _WIN32 || _WIN64
                    std::string new_path = directory.append("\\"+md5_hash+ path_string_ext);
                #endif
                #if __linux__
                    std::string new_path = directory.append("/"+md5_hash+ path_string_ext);
                #endif

                // rename files and print to console
                // on linux, if multiple files have the same hash, they will be renamed to the same file and duplicate will be deleted
                rename(&path_string[0], &new_path[0]);
                std::cout << fs::path(p).filename().string() << " -> " << md5_hash + path_string_ext << std::endl;
                
                /*
                // if there was an error during rename, delete file. (usually happens if there already is a file with the same hash, deleting duplicates)
                int result = rename(&path_string[0], &new_path[0]);
                if (result == 0)
                    //std::cout << path_string << " -> " << new_path << std::endl;
                    
                    std::cout << fs::path(p).filename().string() << " -> " << md5_hash+ path_string_ext << std::endl;
                else {
                    std::cout << "Error renaming file. " << path_string << ". " << new_path << " already exists." << std::endl;
                    std::filesystem::remove(path_string);
                    int_deleted_files += 1;
                }
                */

            }
        }

        // calc total amount of time
        auto t2 = high_resolution_clock::now();
        auto f_secs = std::chrono::duration_cast<std::chrono::duration<float>>(t2 - t1);

        std::cout << std::endl << "Total duration: " << f_secs.count() << " seconds";
        std::cout << "Total amount of deleted files: " << int_deleted_files << std::endl;
    }
	return 0;
	
}
