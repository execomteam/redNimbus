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
            uploadModalShow: false
        }
        
        this.addNewBucket = this.addNewBucket.bind(this);
        this.setCreateModalShow = this.setCreateModalShow.bind(this);
        this.deletingBucket = this.deletingBucket.bind(this);
        this.deletingFile = this.deletingFile.bind(this);

        this.setUploadModalShow = this.setUploadModalShow.bind(this);
        this.uploadFile = this.uploadFile.bind(this);

        this.changePath = this.changePath.bind(this);
        this.enterFolder = this.enterFolder.bind(this);

        const options = {
            headers: { 'token': localStorage.getItem("token")}
        };
        
        
        let path
        if (typeof this.props.path === 'undefined')
            path = "http://localhost:65001/api/bucket";
        else
            path = "http://localhost:65001/api/bucket" + this.props.path;
        
        axios.get(path, options).then(
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

    deletingFile(file) {
        let arr = this.state.files;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] === file.value) {
                arr.splice(i, 1);
            }
        }

        this.setState({ files: arr });
    }

    uploadFile(file) {
        let arr = this.state.files;
        let found = false;
        for (var i = 0; i < arr.length && !found; i++) {
            found = (arr[i] === file.value);
        }

        if (!found) {
            this.setState(prevState => ({
                files: [...prevState.files, file.value]
            }));
        }
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

    changePath(newPath) {
        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };


        this.props.changePath(newPath);
        let path = "http://localhost:65001/api/bucket/post";
        
        axios.post(path, { Path: newPath }, options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    enterFolder(folderName) {
        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };
        let helpPath = this.props.path;
        if (helpPath.slice(-1) === "/") {
            let newPath = this.props.path + folderName + "/";
            this.props.changePath(newPath);
            let path = "http://localhost:65001/api/bucket/post";
            axios.post(path, { Path: newPath }, options).then(
                (resp) => this.onSuccessHandler(resp),
                (resp) => this.onErrorHandler(resp)
            );
        }
    }

    render() {
        return (
            <div className="container">
                <div className="card-body">
                    <h2 className="card-title text-left">{this.props.name+": " + this.props.path}</h2>
                </div>
                <div className="row">
                    <div className="col-md-2">
                        <SideNav
                            path={this.props.path}

                            changePath={this.changePath}

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
                        <FolderGroup deletingBucket={this.deletingBucket} content={this.state.folders} enterFolder={this.enterFolder} path={this.props.path}/>
                        <hr/>
                        <FileGroup deletingFile={this.deletingFile} content={this.state.files} path={this.props.path}/>
                    </div>
                </div>
            </div>
        );
    }
}

export default Bucket;