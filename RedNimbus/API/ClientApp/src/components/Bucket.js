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
            modalShow: false
        }
        
        this.addNewBucket = this.addNewBucket.bind(this);
        this.setModalShow = this.setModalShow.bind(this);

        const options = {
            headers: { 'token': localStorage.getItem("token")}
        };

        axios.get("http://localhost:65001/api/bucket", options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    

    onErrorHandler(response){
        alert("pokriti slucaj");
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
        alert("aaaaa");
        this.setState(prevState => ({
            folders: [...prevState.folders, bucket.value]
        }));
    }

    setModalShow(value){
        this.setState({
            modalShow: value
        });
    }

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-2">
                        <SideNav addNewBucket={this.addNewBucket} modalShow={this.state.modalShow} setModalShow={this.setModalShow} onClick={this.onClickeCreateNewBucket}/>
                    </div>
                    <div className="col-md-10">   
                        <br />
                        <FolderGroup content={this.state.folders}/>
                        <hr/>
                        <FileGroup content={this.state.files}/>
                    </div>
                </div>
            </div>
        );
    }
}

export default Bucket;