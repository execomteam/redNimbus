import React from 'react';
import axios from 'axios';
import FolderGroup from './FolderGroup'
import FileGroup from './FileGroup'
import SideNav from './SideNav'



class Bucket extends React.Component
{
    constructor(props) {
        super(props);

        this.state = {
            folders: [],
            files: [],
            createModalShow: false,
            uploadModalShow: false,
            path: "/"
        }
        
        this.addNewBucket = this.addNewBucket.bind(this);
        this.setCreateModalShow = this.setCreateModalShow.bind(this);
        this.deletingBucket = this.deletingBucket.bind(this);

        this.setUploadModalShow = this.setUploadModalShow.bind(this);
        this.uploadFile = this.uploadFile.bind(this);

        const options = {
            headers: { 'token': localStorage.getItem("token")}
        };

        axios.get("http://localhost:65001/api/bucket", options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    

    onErrorHandler(response){
        alert("Error response: Uncovered case");
    }

    onSuccessHandler(resp){
        var tempFolders = [];
        var tempFiles = [];
        var flag = true;

        for (var i = 0; i < resp.data.length; i++) {
            if (resp.data[i] === '*') {
              flag = false;
              continue;
            }
            if(flag==true){
                tempFolders.push(resp.data[i])
            }else{
                tempFiles.push(resp.data[i]);
            }
        }
        this.setState({
            folders: tempFolders,
            files: tempFiles
        });
    }


    addNewBucket(bucket){
        this.setState(prevState => ({
            folders: [...prevState.folders, bucket.value]
        }));
    }

    deletingBucket(bucket) {
        let arr = this.state.folders;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] === bucket.value) {
                arr.splice(i, 1);
            }
        }

        this.setState({folders: arr});
    }

    uploadFile(file) {
        this.setState(prevState => ({
            files: [...prevState.files, file.value]
        }));
    }

    setCreateModalShow(value){
        this.setState({
            createModalShow: value
        });
    }
    setDeleteModalShow(value) {
        this.setState({
            deleteModalShow: value
        });
    }

    setUploadModalShow(value) {
        this.setState({
            uploadModalShow: value
        });
    }

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-2">
                        <SideNav
                            path={this.state.path}

                            uploadFile={this.uploadFile}
                            uploadModalShow={this.state.uploadModalShow}
                            setUploadModalShow={this.setUploadModalShow}
                           
                            addNewBucket={this.addNewBucket}
                            createModalShow={this.state.createModalShow}
                            setCreateModalShow={this.setCreateModalShow}
                             />
                    </div>
                    <div className="col-md-10">   
                        <br />
                        <FolderGroup deletingBucket={this.deletingBucket} content={this.state.folders}/>
                        <hr/>
                        <FileGroup content={this.state.files}/>
                    </div>
                </div>
            </div>
        );
    }
}

export default Bucket;