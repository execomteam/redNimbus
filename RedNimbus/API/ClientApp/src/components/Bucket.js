import React from 'react';
import axios from 'axios';
import FolderGroup from './FolderGroup'
import FileGroup from './FileGroup'

class Bucket extends React.Component
{
    constructor(props) {
        super(props);

        this.state = {
            folders: ['Volvo', 'BMV'],
            files: ['DilanDog.pdf', 'MarkMystery.txt']
        }

        const options = {
            headers: { 'token': localStorage.getItem("token")}
        };

        axios.get("http://localhost:65001/api/bucket", options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    onErrorHandler(response){
        //will see
    }

    onSuccessHandler(resp){
        var tempFolders = [];
        var tempFiles = [];
        var flag = true;

        for (var i = 0; i < resp.data.length; i++) {
            if (resp.data[i] === '*') {
              flag == false;
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

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-2">
                        <button style={{marginBottom: '20px'}} className="btn btn-danger btn-sm">Crate new</button>
                        <ul className="nav flex-column">
                            <li class="nav-item">
                                <a className="nav-link active" href="#">My Bucket</a>
                            </li>
                            <li className="nav-item">
                                <a className="nav-link" href="#">Shared with me</a>
                            </li>
                        </ul>                      
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